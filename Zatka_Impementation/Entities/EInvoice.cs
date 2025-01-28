namespace Zatka_Impementation_Testing.Entities
{
    public class EInvoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string SellerName { get; set; }
        public string SellerVATNumber { get; set; }
        public string BuyerName { get; set; }
        public string BuyerVATNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VATAmount { get; set; }
        public List<InvoiceItem> Items { get; set; }
    }
}
