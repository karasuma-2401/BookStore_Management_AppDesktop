using System.ComponentModel.DataAnnotations;

namespace BookStoreManagement.API.Models.Invoice
{
    public class InvoiceDetailCreateDto
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }
    }
}
