using BookStoreManagement.API.Models.Report;

namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IReportService
    {
        Task<ReportSummaryDto> GetMonthlyReport(int month, int year);
    }
}