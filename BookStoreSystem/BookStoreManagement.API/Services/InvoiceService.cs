using BookStoreManagement.API.Data;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Models.Enums;
using BookStoreManagement.API.Models.Invoice;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

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

        public async Task<int?> CreateInvoiceAsync(int userId,InvoiceCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var validationResult = await _validator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    var errorMessage = validationResult.Errors.First().ErrorMessage;
                    throw new Exception(errorMessage);
                }

                if (dto.CustomerId.HasValue)
                {
                    var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == dto.CustomerId.Value);
                    if (!customerExists)
                    {
                        throw new Exception($"Customer with ID {dto.CustomerId} does not exist.");
                    }
                }

                var bookIds = dto.Details.Select(d => d.BookId).ToList();
                var books = await _context.Books
                    .Where(b => bookIds.Contains(b.BookId))
                    .ToDictionaryAsync(b => b.BookId);

                if (books.Count != bookIds.Count)
                    throw new Exception("One or more books in the cart do not exist.");

                var invoice = new Invoice
                {
                    CustomerId = dto.CustomerId,
                    UserId = userId,
                    InvoiceDate = DateTime.UtcNow,
                    Status = InvoiceStatus.Completed
                };

                decimal rawTotal = 0;
                var invoiceDetails = new List<InvoiceDetail>();

                foreach (var item in dto.Details)
                {
                    var book = books[item.BookId];

                    if (book.Quantity < item.Quantity)
                    {
                        throw new Exception($"Not enough stock for book '{book.Title}'. Remaining: {book.Quantity}");
                    }

                    book.Quantity -= item.Quantity;

                    var detailTotal = book.Price * item.Quantity;
                    rawTotal += detailTotal;

                    invoiceDetails.Add(new InvoiceDetail
                    {
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        SalePrice = book.Price
                    });
                }

                decimal finalTotal = rawTotal;

                if (!string.IsNullOrEmpty(dto.VoucherCode))
                {
                    var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Code.ToLower() == dto.VoucherCode.ToLower());

                    if (voucher == null)
                        throw new Exception("Voucher code does not exist.");

                    if (voucher.ExpiryDate.HasValue && voucher.ExpiryDate < DateTime.UtcNow)
                        throw new Exception("Voucher code has expired.");

                    if (voucher.UsageLimit.HasValue && voucher.UsedCount >= voucher.UsageLimit)
                        throw new Exception("Voucher usage limit has been reached.");

                    if (voucher.DiscountAmount.HasValue && rawTotal < voucher.DiscountAmount.Value)
                    {
                        throw new Exception($"Order total must be at least {voucher.DiscountAmount.Value:N0}đ to use this voucher.");
                    }

                    if (voucher.DiscountPercent.HasValue)
                    {
                        decimal discount = rawTotal * (voucher.DiscountPercent.Value / 100m);
                        finalTotal = rawTotal - discount;
                    }

                    if (finalTotal < 0) finalTotal = 0;

                    invoice.VoucherId = voucher.VoucherId;
                    voucher.UsedCount += 1;
                }

                invoice.Total = finalTotal;
                invoice.InvoiceDetails = invoiceDetails;


                _context.Invoices.Add(invoice);

                if (dto.CustomerId.HasValue)
                {
                    var customer = await _context.Customers.FindAsync(dto.CustomerId.Value);
                    if (customer != null)
                    {
                        customer.Debt += finalTotal;
                        _context.Customers.Update(customer);
                    }
                }
                else
                {
                    var payment = new Payment
                    {
                        CustomerId = dto.CustomerId,
                        UserId = userId,
                        Amount = finalTotal,
                        PaymentDate = DateTime.UtcNow,
                        Invoice = invoice
                    };
                    _context.Payments.Add(payment);
                }

                var result = await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return result > 0 ? invoice.InvoiceId : null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Checkout Error: " + ex.Message);
            }
        }

        public async Task<List<InvoiceListDto>> GetAllInvoicesAsync()
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                .OrderByDescending(i => i.InvoiceDate)
                .Select(i => new InvoiceListDto
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceDate = i.InvoiceDate,
                    Total = i.Total,
                    CustomerName = i.Customer != null ? i.Customer.Name : "Guest",
                    StaffName = i.User.FullName,
                    TotalItems = i.InvoiceDetails.Sum(d => d.Quantity),
                    Status = i.Status.ToString()
                })
                .ToListAsync();
        }

        public async Task<InvoiceDetailResponseDto?> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.User)
                .Include(i => i.Voucher)
                .Include(i => i.InvoiceDetails).ThenInclude(d => d.Book)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null) return null;

            var paidAmount = await _context.Payments
                .Where(p => p.InvoiceId == id)
                .SumAsync(p => p.Amount);

            return new InvoiceDetailResponseDto
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceDate = invoice.InvoiceDate,
                Total = invoice.Total,
                CustomerName = invoice.Customer != null ? invoice.Customer.Name : "Guest",
                StaffName = invoice.User.FullName,
                VoucherCode = invoice.Voucher != null ? invoice.Voucher.Code : null,
                TotalItems = invoice.InvoiceDetails.Sum(d => d.Quantity),
                Status = invoice.Status.ToString(),
                PaidAmount = paidAmount,
                RemainingAmount = invoice.Total - paidAmount,
                Items = invoice.InvoiceDetails.Select(d => new InvoiceItemDto
                {
                    BookTitle = d.Book != null ? d.Book.Title : "Unknown Book",
                    Quantity = d.Quantity,
                    SalePrice = d.SalePrice
                }).ToList()
            };
        }

        public async Task<bool> CancelInvoiceAsync(int id)
        {

            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null || invoice.Status == InvoiceStatus.Canceled)
                return false;

            invoice.Status = InvoiceStatus.Canceled;

            foreach (var detail in invoice.InvoiceDetails)
            {
                var book = await _context.Books.FindAsync(detail.BookId);
                if (book != null)
                {
                    book.Quantity += detail.Quantity;
                }
                else
                {
                    Console.WriteLine($"Warning: Book with ID {detail.BookId} no longer exists. Inventory not restored.");
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }

    }
}
