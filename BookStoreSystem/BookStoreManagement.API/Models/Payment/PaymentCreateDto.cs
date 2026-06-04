using System.ComponentModel.DataAnnotations;

namespace BookStoreManagement.API.Models.Payment
{
    public class PaymentCreateDto
    {
        [Required]
        public int InvoiceId { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}