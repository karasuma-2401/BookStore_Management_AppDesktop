using BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API;

public interface IInvoiceExportService
{
    Task<bool> ExportInvoiceToExcelAsync(InvoiceDetailResponseDto invoice);
    Task<bool> ExportPaymentHistoryToExcelAsync(InvoiceDetailResponseDto invoice, System.Collections.Generic.List<PaymentResponseDto> payments);
}