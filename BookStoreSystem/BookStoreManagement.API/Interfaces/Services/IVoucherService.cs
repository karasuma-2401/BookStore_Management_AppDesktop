using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Models.Voucher;

namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IVoucherService
    {
        Task<IEnumerable<Voucher>> GetAllAsync();
        Task<Voucher> CreateAsync(VoucherCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
