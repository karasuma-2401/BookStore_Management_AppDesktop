using BookStoreManagement.API.Models.Invoice;

namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IInvoiceService
    {
        Task<int?> CreateInvoiceAsync(int userId, InvoiceCreateDto dto);
        Task<List<InvoiceListDto>> GetAllInvoicesAsync();
        Task<InvoiceDetailResponseDto?> GetInvoiceByIdAsync(int id);
        Task<bool> CancelInvoiceAsync(int id);
    }
}
