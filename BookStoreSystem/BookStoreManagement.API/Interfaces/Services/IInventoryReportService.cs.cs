using BookStoreManagement.API.Models.DTOs;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Services.Interfaces
{
    public interface IInventoryReportService
    {
        Task<InventoryReport> CreateReport(int month, int year, int bookId);
        Task<IEnumerable<InventoryReportResponseDTO>> GetReports(int month, int year);
        Task<InventoryReportResponseDTO?> GetById(int id);
        Task<IEnumerable<InventoryReportResponseDTO>> GetByBook(int bookId);
        Task GenerateReport(int month, int year);
    }
}