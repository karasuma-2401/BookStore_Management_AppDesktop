namespace BookStoreManagement.API.Models.DTOs
{
    public class CreateDebtReportDTO
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int CustomerId { get; set; }
    }
}