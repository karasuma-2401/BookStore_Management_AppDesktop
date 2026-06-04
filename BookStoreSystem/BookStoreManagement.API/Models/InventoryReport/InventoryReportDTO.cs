namespace BookStoreManagement.API.Models.DTOs
{
    public class CreateInventoryReportDTO
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int BookId { get; set; }
    }
}