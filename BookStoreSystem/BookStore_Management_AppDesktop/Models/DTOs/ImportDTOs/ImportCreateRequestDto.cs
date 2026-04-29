namespace BookStore_Management_AppDesktop.Models.DTOs.ImportDTOs
{
    public class ImportCreateRequestDto
    {
        public List<ImportDetailRequestDto> Details { get; set; } = new List<ImportDetailRequestDto>();
    }

    public class ImportDetailRequestDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
    }
}
