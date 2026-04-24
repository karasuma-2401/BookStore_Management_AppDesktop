using BookStoreManagement.API.Data;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Models.Invoice;
using FluentValidation;

namespace BookStoreManagement.API.Services
{
    public class InvoiceService : IInvoiceService
    {

        private readonly ApplicationDBContext _context;
        private readonly IValidator<InvoiceCreateDto> _validator;

        public InvoiceService(ApplicationDBContext context, IValidator<InvoiceCreateDto> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<int?> CreateInvoiceAsync(InvoiceCreateDto dto)
        {

            var bookIds = dto.Details.Select(d => d.BookId).ToList();
         

            

            var invoice = new Invoice
            {
                CustomerId = dto.CustomerId,
                UserId = dto.UserId,
                InvoiceDate = DateTime.UtcNow
            };

            decimal rawTotal = 0;
            var invoiceDetails = new List<InvoiceDetail>();

  

            decimal finalTotal = rawTotal;

            

            var payment = new Payment
            {
                CustomerId = dto.CustomerId ?? 0,
                UserId = dto.UserId,
                Amount = finalTotal,
                PaymentDate = DateTime.UtcNow,
                Invoice = invoice
            };

            _context.Invoices.Add(invoice);
            _context.Payments.Add(payment);

            var result = await _context.SaveChangesAsync();

            return result > 0 ? invoice.InvoiceId : null;
        }

    }
}
