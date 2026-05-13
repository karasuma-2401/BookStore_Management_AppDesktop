using System.ComponentModel.DataAnnotations;

namespace BookStoreManagement.API.Models.Report
{
    public class ReportSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalImportCost { get; set; }
        public decimal Profit { get; set; }
        public int TotalBooksSold { get; set; }

        public List<DailyRevenueDto> DailyData { get; set; } = new();
        public List<TopBookDto> TopSellingBooks { get; set; } = new();
    }
}