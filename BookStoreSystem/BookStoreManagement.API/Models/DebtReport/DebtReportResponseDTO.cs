namespace BookStoreManagement.API.Models.DTOs
{
    public class DebtReportResponseDTO
    {
        public int ReportId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int CustomerId { get; set; }

        public decimal OpeningDebt { get; set; }
        public decimal ChangeAmount { get; set; }
        public decimal ClosingDebt { get; set; }
    }
}