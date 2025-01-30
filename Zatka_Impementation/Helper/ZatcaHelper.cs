using System.Net.Http.Json;
using System.Text.Json;
using System.Xml;
using Zatca.EInvoice.SDK;
using Zatca.EInvoice.SDK.Contracts;
namespace Zatka_Impementation_Testing.Helper
{
    public class ZatcaHelper
    {

        public static string SignInvoice(string xmlContent)
        {
            
            string privateKeyPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "private_key.pem");
            string certificatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "certificate.pem");

            string privateKeyContent = File.ReadAllText(privateKeyPath);
            string certificateContent = File.ReadAllText(certificatePath);

            // Convert the XML content to XmlDocument
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlContent);

            // Use the SDK's signer class
            var signer = new EInvoiceSigner(); // Ensure to use the correct namespace and class from the SDK
            var signResult = signer.SignDocument(xmlDocument, certificateContent, privateKeyContent);

            // Return the signed invoice
            return signResult.SignedEInvoice.OuterXml;
        }

        public static string GenerateQrCode(string xmlContent)
        {
            try
            {
                // Load the XML content into an XmlDocument
                XmlDocument invoiceXml = new XmlDocument();
                invoiceXml.LoadXml(xmlContent);

                // Create an instance of the QR Code generator
                var qrGenerator = new EInvoiceQRGenerator();

                // Generate the QR Code
                var qrResult = qrGenerator.GenerateEInvoiceQRCode(invoiceXml);

                // Check if the QR generation was successful
                if (qrResult.IsValid)
                {
                    return qrResult.QR; // Return the Base64 encoded QR code
                }
                else
                {
                    throw new Exception($"QR Code generation failed: {qrResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating QR Code: {ex.Message}");
                throw;
            }
        }

        public static bool ValidateInvoice(string jsonContent)
        {
            try
            {
                var json = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                string xmlContent = json["signedInvoice"];
                // Load the XML content
                XmlDocument invoiceXml = new XmlDocument();
                invoiceXml.LoadXml(xmlContent);

                // Load your certificate and PIH (if required)
                string certificatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/certificate.pem");
                string certificateContent = File.ReadAllText(certificatePath);

                // Validator instance
                var validator = new EInvoiceValidator(); // Replace with the correct SDK class
                var validationResult = validator.ValidateEInvoice(invoiceXml, certificateContent, null);
                if (!validationResult.IsValid)
                {
                    Console.WriteLine("Validation Failed:");
                    foreach (var error in validationResult.ValidationSteps)
                    {
                        Console.WriteLine(error);
                    }
                }
                // Check if the invoice is valid
                return validationResult.IsValid; // Returns true if the invoice is valid
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
