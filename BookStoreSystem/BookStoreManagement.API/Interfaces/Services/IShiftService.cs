using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Models.Shift;

namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IShiftService
    {
        Task<IEnumerable<Shift>> GetAllShiftsAsync();
        Task<Shift?> GetShiftByIdAsync(int id);
        Task<string?> UpdateShiftTimeAsync(int id, ShiftTimeUpdateDto dto);
    }
}
