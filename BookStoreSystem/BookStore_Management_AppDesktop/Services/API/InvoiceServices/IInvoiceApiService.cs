using BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.InvoiceServices
{
    public interface IInvoiceApiService
    {
        Task<List<InvoiceListDto>> GetAllInvoicesAsync();
        Task<InvoiceDetailResponseDto?> GetInvoiceByIdAsync(int id);
        Task<bool> CancelInvoiceAsync(int id);
        Task<int?> CreateInvoiceAsync(InvoiceCreateDto invoiceCreateDto);
        Task<bool> RecordPaymentAsync(int invoiceId, decimal amount);
        Task<List<PaymentResponseDto>> GetPaymentsByInvoiceIdAsync(int invoiceId);
    }
}
