using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.qrcode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Xml.Linq;
using Zatka_Impementation_Testing.Entities;
using System.Text;
using System.Security.Cryptography;
using Zatka_Impementation_Testing.Helper;

namespace Zatka_Impementation_Testing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InvoiceController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("generate-xml-invoice")]
        public IActionResult GenerateEInvoiceXml(EInvoice invoice)
        {
            try
            {
                #region Dummy data

                invoice.InvoiceNumber = "INV-2025-0025";
                invoice.InvoiceDate = DateTime.Now;
                invoice.SellerName = "ABC Company";
                invoice.SellerVATNumber = "123456789";
                invoice.BuyerName = "XYZ Customer";
                invoice.BuyerVATNumber = "987654321";
                invoice.TotalAmount = 350.00m;
                invoice.VATAmount = 0.15m;

                // Create dummy invoice items
                invoice.Items = new List<InvoiceItem>
            {
                new InvoiceItem
                {
                    ItemName = "Product A",
                    //Quantity = 2,
                    //UnitPrice = 100.00m,
                    //TotalPrice = 200.00m
                },
                new InvoiceItem
                {
                    ItemName = "Product B",
                    //Quantity = 1,
                    //UnitPrice = 150.00m,
                    //TotalPrice = 150.00m
                }
            };

                // Calculate total amount and VAT amount
                //decimal totalAmount = items.Sum(item => item.TotalPrice); // Sum of all item totals
                //decimal vatRate = 0.15m; // 15% VAT rate
                //decimal vatAmount = totalAmount * vatRate;

                #endregion
                XNamespace ns = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
                XDocument doc = new XDocument(
                    new XElement(ns + "Invoice",
                        new XElement(ns + "ID", invoice.InvoiceNumber),
                        new XElement(ns + "IssueDate", invoice.InvoiceDate.ToString("yyyy-MM-dd")),
                        new XElement(ns + "AccountingSupplierParty",
                            new XElement(ns + "Party",
                                new XElement(ns + "PartyName", invoice.SellerName),
                                new XElement(ns + "CompanyID", invoice.SellerVATNumber)
                            )
                        ),
                        new XElement(ns + "AccountingCustomerParty",
                            new XElement(ns + "Party",
                                new XElement(ns + "PartyName", invoice.BuyerName),
                                new XElement(ns + "CompanyID", invoice.BuyerVATNumber)
                            )
                        ),
                        new XElement(ns + "TaxTotal",
                            new XElement(ns + "TaxAmount", invoice.VATAmount)
                        ),
                        new XElement(ns + "LegalMonetaryTotal",
                            new XElement(ns + "LineExtensionAmount", invoice.TotalAmount)
                        )
                    )
                );

                //return doc.ToString();
                //return Ok(doc.ToString());
                //return Content(doc.ToString(), "application/xml");
                // Convert XML to String
                //string xmlString = doc.ToString();

                // Load the Private Key from wwwroot
                //using RSA rsa = LoadPrivateKey();

                // Sign the XML Invoice
                //string signedInvoice = SignInvoiceXml(xmlString, rsa);
                string signedInvoice = ZatcaHelper.SignInvoice(doc.ToString());

                // Return Signed Invoice
                return Ok(new { SignedInvoice = signedInvoice });
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        [HttpGet("validate-invoice")]
        public IActionResult ValidateInvoice(string xmlInvoice)
        {
            try
            {
                var result = ZatcaHelper.ValidateInvoice(xmlInvoice);
                return Ok(result);
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }


        [HttpGet("submit-invoice")]
        public async Task<string> SubmitInvoiceToZATCA(string xmlInvoice, string apiUrl, string apiKey)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var content = new StringContent(xmlInvoice, Encoding.UTF8, "application/xml");
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }
    }
}
