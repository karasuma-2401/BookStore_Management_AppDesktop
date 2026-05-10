using BookStore_Management_AppDesktop.Models.DTOs.ReportDTOs;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.ReportServices
{
    public interface IReportApiService
    {
        Task<ReportSummaryDto?> GetMonthlyReportAsync(int month, int year);
    }
}