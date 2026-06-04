using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Services.Interfaces
{
    public interface IDebtReportService
    {
        Task<DebtReport> CreateReport(int month, int year, int customerId);
    }
}