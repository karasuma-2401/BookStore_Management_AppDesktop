using BookStoreManagement.API.Models.Invoice;

namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IInvoiceService
    {
        Task<int?> CreateInvoiceAsync(InvoiceCreateDto dto);
    }
}
