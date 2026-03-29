using BookStoreManagement.API.Data;
using BookStoreManagement.API.Models.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using BookStoreManagement.API.Models.Shift;

namespace BookStoreManagement.API.Interfaces.Services
{
    public class ShiftService : IShiftService
    {

        private readonly ApplicationDBContext _context;
        private readonly IValidator<ShiftTimeUpdateDto> _timeValidator;

        public ShiftService(ApplicationDBContext context, IValidator<ShiftTimeUpdateDto> validator)
        {
            _context = context;
            _timeValidator = validator;
        }

        public async Task<IEnumerable<Shift>> GetAllShiftsAsync()
        {
            return await _context.Shifts.OrderBy(s => s.StartTime).ToListAsync();
        }

        public async Task<Shift?> GetShiftByIdAsync(int id)
        {
            return await _context.Shifts.FindAsync(id);
        }

        public async Task<string?> UpdateShiftTimeAsync(int id, ShiftTimeUpdateDto dto)
        {

            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null) return "Shift not found.";

            if (dto.StartTime >= dto.EndTime)
                return "Start time must be earlier than end time.";

            shift.StartTime = dto.StartTime;
            shift.EndTime = dto.EndTime;

            try
            {
                _context.Shifts.Update(shift);
                await _context.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                return $"Database error: {ex.Message}";
            }
        }

    }
}
