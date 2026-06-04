using System;

namespace BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string StaffName { get; set; } = null!;
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
    }
}
