using System.Collections.Generic;

namespace BookStore_Management_AppDesktop.Models.DTOs.ReportDTOs
{
    public class ReportSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalImportCost { get; set; }
        public decimal Profit { get; set; }
        public int TotalBooksSold { get; set; }
        public List<DailyRevenueDto> DailyData { get; set; } = new List<DailyRevenueDto>();
        public List<TopBookDto> TopSellingBooks { get; set; } = new List<TopBookDto>();
    }

    public class DailyRevenueDto
    {
        public int Day { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopBookDto
    {
        public string Title { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal RevenueGenerated { get; set; }
    }
}