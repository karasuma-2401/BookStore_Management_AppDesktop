using BookStoreManagement.API.Models.DTOs;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Services.Interfaces
{
    public interface IDebtReportService
    {
        Task<DebtReport> CreateReport(int month, int year, int customerId);
        Task<IEnumerable<DebtReportResponseDTO>> GetReports(int month, int year);
        Task<DebtReportResponseDTO?> GetById(int id);
        Task<IEnumerable<DebtReportResponseDTO>> GetByCustomer(int customerId);
    }
}