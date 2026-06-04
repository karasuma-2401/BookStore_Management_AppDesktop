using BookStoreManagement.API.Data;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Models.Voucher;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly ApplicationDBContext _context;
        private readonly IValidator<VoucherCreateDto> _validator;

        public VoucherService(ApplicationDBContext context, IValidator<VoucherCreateDto> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<IEnumerable<Voucher>> GetAllAsync()
        {
            return await _context.Vouchers
                .OrderByDescending(v => v.ExpiryDate)
                .ToListAsync();
        }

        public async Task<Voucher> CreateAsync(VoucherCreateDto dto)
        {
  
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new Exception(validationResult.Errors.First().ErrorMessage);
            }

            var normalizedCode = dto.Code.ToUpper().Trim();
            if (await _context.Vouchers.AnyAsync(v => v.Code == normalizedCode))
            {
                throw new Exception("Voucher code already exists.");
            }

            var voucher = new Voucher
            {
                Code = normalizedCode,
                DiscountPercent = dto.DiscountPercent,
                DiscountAmount = dto.DiscountAmount,
                ExpiryDate = dto.ExpiryDate.HasValue ? DateTime.SpecifyKind(dto.ExpiryDate.Value.Date, DateTimeKind.Utc) : null,
                UsageLimit = dto.UsageLimit,
                UsedCount = 0
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            return voucher;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null) return false;

            try
            {
                _context.Vouchers.Remove(voucher);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException)
            {
                // Reset EF Core state for this entity
                _context.Entry(voucher).State = EntityState.Unchanged;

                // Soft-deactivate by setting expiry date to yesterday and capping limit
                voucher.ExpiryDate = DateTime.UtcNow.AddDays(-1);
                voucher.UsageLimit = voucher.UsedCount;
                await _context.SaveChangesAsync();

                throw new InvalidOperationException("Voucher has already been used in invoices. It cannot be deleted from history, but it has been deactivated & expired successfully!");
            }
        }
    }
}
