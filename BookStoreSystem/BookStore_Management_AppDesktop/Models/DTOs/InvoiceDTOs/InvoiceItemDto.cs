namespace BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs
{
    public class InvoiceItemDto
    {
        public string BookTitle { get; set; } = null!; // Tên sách
        public int Quantity { get; set; }              // Khớp quantity trong ảnh
        public decimal SalePrice { get; set; }         // Khớp sale_price trong ảnh
        public decimal SubTotal { get; set; }
    }
}
