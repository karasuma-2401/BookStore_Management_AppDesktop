using System.ComponentModel.DataAnnotations;

namespace BookStoreManagement.API.Models.Report
{
    public class DailyRevenueDto
    {
        public int Day { get; set; }
        public decimal Revenue { get; set; }
    }
}