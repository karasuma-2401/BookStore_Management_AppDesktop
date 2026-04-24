namespace BookStoreManagement.API.Models.Invoice
{
    public class InvoiceListDto
    {
        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Total { get; set; }
        public string? CustomerName { get; set; }
        public string? StaffName { get; set; }
        public int TotalItems { get; set; }
        public string Status { get; set; } = null!;
    }
}
