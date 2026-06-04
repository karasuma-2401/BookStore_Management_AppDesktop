using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Services.Interfaces
{
    public interface IInventoryReportService
    {
        Task<InventoryReport> CreateReport(int month, int year, int bookId);
    }
}