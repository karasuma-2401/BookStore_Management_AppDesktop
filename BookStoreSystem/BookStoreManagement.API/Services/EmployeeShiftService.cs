using BookStoreManagement.API.Data;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Models.Shift;
using BookStoreManagement.API.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Services
{
    public class EmployeeShiftService : IEmployeeShift
    {
        private readonly ApplicationDBContext _context;
        private readonly IValidator<ShiftAssignDto> _validator;

        public EmployeeShiftService(ApplicationDBContext context, IValidator<ShiftAssignDto> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<string?> AssignShiftAsync(ShiftAssignDto dto)
        {

            if (!await _context.Shifts.AnyAsync(s => s.ShiftId == dto.ShiftId))
                return "Shift not found.";

            if (!await _context.Employees.AnyAsync(e => e.EmployeeId == dto.EmployeeId))
                return "Employee not found.";

            if (await _context.EmployeeShifts.AnyAsync(es => es.EmployeeId == dto.EmployeeId && es.WorkDate == dto.WorkDate))
                return "Employee already has a shift assigned for this date.";

            var employeeshift = new EmployeeShift
            {
                EmployeeId = dto.EmployeeId,
                ShiftId = dto.ShiftId,
                WorkDate = dto.WorkDate,
            };

            _context.EmployeeShifts.Add(employeeshift);
            var result = await _context.SaveChangesAsync();
            return result > 0 ? null : "Could not save the assignment to database.";


        }
        public async Task<IEnumerable<EmployeeShiftResponseDto>> GetScheduleAsync(DateTime startDate, DateTime endDate, int? employeeId = null)
        {
            var start = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
            var end = DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc);

            var query = _context.EmployeeShifts
            .Include(es => es.Employee)
            .Include(es => es.Shift)
            .Where(es => es.WorkDate >= start && es.WorkDate <= end);

            if (employeeId.HasValue)
            {
                query = query.Where(es => es.EmployeeId == employeeId.Value);
            }

            return await query
                .Select(es => new EmployeeShiftResponseDto
                {
                    Id = es.Id,
                    FullName = es.Employee.FullName,
                    ShiftName = es.Shift.ShiftName,
                    WorkTime = $"{es.Shift.StartTime} - {es.Shift.EndTime}",
                    WorkDate = es.WorkDate.ToString("yyyy-MM-dd")
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            var assignment = await _context.EmployeeShifts.FindAsync(id);
            if (assignment == null) return false;

            _context.EmployeeShifts.Remove(assignment);
            return await _context.SaveChangesAsync() > 0;
        }


    }
}
