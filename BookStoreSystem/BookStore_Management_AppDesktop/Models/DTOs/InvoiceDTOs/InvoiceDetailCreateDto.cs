namespace BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs
{
    public class InvoiceDetailCreateDto
    {
        public int BookId { get; set; }  // ID sách để DB biết là cuốn nào
        public int Quantity { get; set; } // Số lượng mua
    }
}