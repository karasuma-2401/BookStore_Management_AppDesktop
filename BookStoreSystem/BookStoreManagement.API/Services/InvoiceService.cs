using BookStoreManagement.API.Data;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Models.Enums;
using BookStoreManagement.API.Models.Invoice;
using BookStoreManagement.API.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDBContext _context;
        private readonly IValidator<InvoiceCreateDto> _validator;
        private readonly ISettingService _settingService;

        public InvoiceService(ApplicationDBContext context, IValidator<InvoiceCreateDto> validator, ISettingService settingService)
        {
            _context = context;
            _validator = validator;
            _settingService = settingService;
        }

        public async Task<int?> CreateInvoiceAsync(int userId,InvoiceCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var maxDebt = await _settingService.GetDecimal("NOTOIDA");
                var minStockAfterSale = await _settingService.GetInt("SLTONSAUBAN");
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

                if (dto.CustomerId.HasValue)
                {
                    var customer = await _context.Customers.FindAsync(dto.CustomerId.Value);

                    if (customer != null && customer.Debt > maxDebt)
                        throw new Exception($"Customer debt exceeds limit ({maxDebt})");
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
                    if ((book.Quantity - item.Quantity) < minStockAfterSale)
                        throw new Exception($"Stock after sale must be at least {minStockAfterSale}");

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

                    decimal discount = 0;
                    if (voucher.DiscountPercent.HasValue)
                    {
                        discount = rawTotal * (voucher.DiscountPercent.Value / 100m);
                    }
                    else if (voucher.DiscountAmount.HasValue)
                    {
                        discount = voucher.DiscountAmount.Value;
                    }

                    finalTotal = rawTotal - discount;

                    if (finalTotal < 0) finalTotal = 0;

                    invoice.VoucherId = voucher.VoucherId;
                    voucher.UsedCount += 1;
                }

                if (dto.CustomerId.HasValue)
                {
                    invoice.Status = InvoiceStatus.Unpaid;
                    invoice.AmountPaid = 0;
                    var customer = await _context.Customers.FindAsync(dto.CustomerId.Value);
                    if (customer != null)
                    {
                        customer.Debt += finalTotal;
                    }
                }
                else
                {
                    invoice.Status = InvoiceStatus.Completed;
                    invoice.AmountPaid = finalTotal;
                }
                invoice.Total = finalTotal;
                invoice.InvoiceDetails = invoiceDetails;

                _context.Invoices.Add(invoice);
                var result = await _context.SaveChangesAsync();

                if (!dto.CustomerId.HasValue)
                {
                    var payment = new Payment
                    {
                        CustomerId = null,
                        InvoiceId = invoice.InvoiceId,
                        UserId = userId,
                        Amount = finalTotal,
                        PaymentDate = DateTime.UtcNow
                    };
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return result > 0 ? (int?)invoice.InvoiceId : null;
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
                    .ThenInclude(u => u.Employee)
                .Include(i => i.InvoiceDetails)
                .OrderByDescending(i => i.InvoiceDate)
                .Select(i => new InvoiceListDto
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceDate = i.InvoiceDate,
                    Total = i.Total,
                    CustomerName = i.Customer != null ? i.Customer.Name : "Guest",
                    CustomerId = i.CustomerId,
                    StaffName = i.User.Employee != null ? i.User.Employee.FullName : i.User.Username,
                    StaffRole = i.User.RoleId,
                    TotalItems = i.InvoiceDetails.Sum(d => d.Quantity),
                    Status = i.Status.ToString()
                })
                .ToListAsync();
        }

        public async Task<InvoiceDetailResponseDto?> GetInvoiceByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.User)
                    .ThenInclude(u => u.Employee)
                .Include(i => i.Voucher)
                .Include(i => i.InvoiceDetails).ThenInclude(d => d.Book)
                .Where(i => i.InvoiceId == id)
                .Select(i => new InvoiceDetailResponseDto
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceDate = i.InvoiceDate,
                    Total = i.Total,
                    CustomerName = i.Customer != null ? i.Customer.Name : "Guest",
                    CustomerId = i.CustomerId,
                    StaffName = i.User.Employee != null ? i.User.Employee.FullName : i.User.Username,
                    StaffRole = i.User.RoleId,
                    VoucherCode = i.Voucher != null ? i.Voucher.Code : null,
                    TotalItems = i.InvoiceDetails.Sum(d => d.Quantity),
                    PaidAmount = i.AmountPaid,
                    RemainingAmount = i.Total - i.AmountPaid,
                    Items = i.InvoiceDetails.Select(d => new InvoiceItemDto
                    {
                        BookTitle = d.Book != null ? d.Book.Title : "Unknown Book",
                        Quantity = d.Quantity,
                        SalePrice = d.SalePrice
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CancelInvoiceAsync(int id)
        {
 
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.Customer)
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

            if (invoice.Customer != null)
            {
                decimal remainingDebt = invoice.Total - invoice.AmountPaid;

                if (remainingDebt > 0)
                {
                    invoice.Customer.Debt -= remainingDebt;

                    if (invoice.Customer.Debt < 0)
                    {
                        invoice.Customer.Debt = 0;
                    }
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }

    }
}
