namespace BookStoreManagement.API.Models.DTOs
{
    public class InventoryReportResponseDTO
    {
        public int ReportId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int BookId { get; set; }
        public string BookName { get; set; } = "";
        public int OpeningStock { get; set; }
        public int ChangeAmount { get; set; }
        public int ClosingStock { get; set; }
    }
}