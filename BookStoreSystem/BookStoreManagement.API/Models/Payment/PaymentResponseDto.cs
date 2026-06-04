namespace BookStoreManagement.API.Models.Payment
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public string? CustomerName { get; set; }
        public string? StaffName { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
    }
}