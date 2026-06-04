using BookStoreManagement.API.Models.Payment;

namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<bool> CreatePaymentAsync(int userId, PaymentCreateDto dto);
        Task<List<PaymentResponseDto>> GetPaymentsByInvoiceIdAsync(int invoiceId);
        Task<bool> CancelPaymentAsync(int paymentId);
    }
}