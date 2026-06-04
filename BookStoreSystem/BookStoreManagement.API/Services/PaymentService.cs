using BookStoreManagement.API.Data;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Models.Enums;
using BookStoreManagement.API.Models.Payment;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDBContext _context;
        private readonly IValidator<PaymentCreateDto> _validator;

        public PaymentService(ApplicationDBContext context, IValidator<PaymentCreateDto> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<bool> CreatePaymentAsync(int userId, PaymentCreateDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errorMessage = validationResult.Errors.First().ErrorMessage;
                throw new Exception(errorMessage);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var invoice = await _context.Invoices
                    .Include(i => i.Customer)
                    .FirstOrDefaultAsync(i => i.InvoiceId == dto.InvoiceId);

                if (invoice == null)
                    throw new Exception("Invoice not found.");

                if (invoice.Status == InvoiceStatus.Completed)
                    throw new Exception("This invoice is already fully paid.");


                decimal remainingAmount = invoice.Total - invoice.AmountPaid;
                if (dto.Amount > remainingAmount)
                {
                    throw new Exception($"Payment amount ({dto.Amount}) exceeds the remaining debt ({remainingAmount}).");
                }

                var payment = new Payment
                {
                    CustomerId = invoice.CustomerId,
                    InvoiceId = invoice.InvoiceId,
                    UserId = userId,
                    Amount = dto.Amount,
                    PaymentDate = DateTime.UtcNow
                };

                _context.Payments.Add(payment);

                invoice.AmountPaid += dto.Amount;

                if (invoice.AmountPaid >= invoice.Total)
                    invoice.Status = InvoiceStatus.Completed;
                else
                    invoice.Status = InvoiceStatus.Partial;

                if (invoice.Customer != null)
                {
                    invoice.Customer.Debt -= dto.Amount;

                    if (invoice.Customer.Debt < 0)
                    {
                        invoice.Customer.Debt = 0;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Payment Error: " + ex.Message);
            }
        }
        public async Task<List<PaymentResponseDto>> GetPaymentsByInvoiceIdAsync(int invoiceId)
        {
            return await _context.Payments
                .Include(p => p.Customer)
                .Include(p => p.User)
                    .ThenInclude(u => u.Employee)
                .Where(p => p.InvoiceId == invoiceId)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new PaymentResponseDto
                {
                    PaymentId = p.PaymentId,
                    InvoiceId = p.InvoiceId,
                    CustomerName = p.Customer != null ? p.Customer.Name : "Guest",
                    StaffName = p.User != null && p.User.Employee != null ? p.User.Employee.FullName : (p.User != null ? p.User.Username : "Unknown"),
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount
                })
                .ToListAsync();
        }

        public async Task<bool> CancelPaymentAsync(int paymentId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var payment = await _context.Payments
                    .Include(p => p.Invoice)
                        .ThenInclude(i => i.Customer)
                    .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

                if (payment == null)
                    return false;

                var invoice = payment.Invoice;


                invoice.AmountPaid -= payment.Amount;
                if (invoice.AmountPaid < 0) invoice.AmountPaid = 0;


                if (invoice.AmountPaid == 0)
                    invoice.Status = InvoiceStatus.Unpaid;
                else if (invoice.AmountPaid < invoice.Total)
                    invoice.Status = InvoiceStatus.Partial;

                if (invoice.Customer != null)
                {
                    invoice.Customer.Debt += payment.Amount;
                }

                _context.Payments.Remove(payment);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();



                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Cancel Payment Error: " + ex.Message);
            }
        }

    }

}