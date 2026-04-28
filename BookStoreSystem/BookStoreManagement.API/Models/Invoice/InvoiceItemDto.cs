namespace BookStoreManagement.API.Models.Invoice
{
    public class InvoiceItemDto
    {
        public string BookTitle { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal SubTotal => Quantity * SalePrice;
    }
}
