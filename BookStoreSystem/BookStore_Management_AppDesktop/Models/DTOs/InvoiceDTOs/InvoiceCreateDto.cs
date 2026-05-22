using System.ComponentModel.DataAnnotations;

namespace BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs
{
    public class InvoiceCreateDto
    {
        public int? CustomerId { get; set; }
        public int? UserId { get; set; }
        public string? VoucherCode { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Cart cannot be empty.")]
        public List<InvoiceDetailCreateDto> Details { get; set; } = new();
    }
}
