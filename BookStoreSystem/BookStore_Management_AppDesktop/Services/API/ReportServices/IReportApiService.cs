using BookStore_Management_AppDesktop.Models.DTOs.ReportDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.ReportServices
{
    public interface IReportApiService
    {
        Task<ReportSummaryDto?> GetMonthlyReportAsync(int month, int year);
        Task<IEnumerable<InventoryReportResponseDTO>> GetInventoryReportsAsync(int month, int year);
        Task<IEnumerable<DebtReportResponseDTO>> GetDebtReportsAsync(int month, int year);
        Task GenerateInventoryReportAsync(int month, int year);
        Task GenerateDebtReportAsync(int month, int year);
    }
}