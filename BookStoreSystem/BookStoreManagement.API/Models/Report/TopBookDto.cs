using System.ComponentModel.DataAnnotations;

namespace BookStoreManagement.API.Models.Report
{
    public class TopBookDto
    {
        public string Title { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal RevenueGenerated { get; set; }
    }
}