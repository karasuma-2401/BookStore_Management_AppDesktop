using BookStore_Management_AppDesktop.Models.DTOs.ReportDTOs;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.Export
{
    public interface IExportService
    {
        Task<bool> ExportReportToExcelAsync(
            int month,
            int year,
            decimal totalRevenue,
            decimal totalImportCost,
            decimal profit,
            int totalBooksSold,
            ObservableCollection<TopBookDto> topBooks);
    }
}
