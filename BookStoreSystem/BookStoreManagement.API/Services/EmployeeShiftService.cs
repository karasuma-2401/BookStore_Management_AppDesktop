using BookStoreManagement.API.Data;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Models.Shift;
using BookStoreManagement.API.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;

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
                Status = "Scheduled",
                CheckInTime = null,
                IsPaid = true
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
                    WorkDate = es.WorkDate.ToString("yyyy-MM-dd"),
                    Status = es.Status,
                    CheckInTime = es.CheckInTime,
                    IsPaid = es.IsPaid
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

        public async Task<string?> CheckInAsync(int assignmentId, int currentUserId)
        {

            var assignment = await _context.EmployeeShifts
            .Include(es => es.Employee)
            .Include(es => es.Shift)
            .FirstOrDefaultAsync(es => es.Id == assignmentId);

            if (assignment == null) return "Assignment not found.";

            if (assignment.Employee.UserId != currentUserId)
                return "You do not have permission to check in for this shift.";

            if (assignment.Status != "Scheduled")
                return "Cannot check in. Current status is " + assignment.Status;

            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime nowVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);


            DateTime shiftStartTime = assignment.WorkDate.Date.Add(assignment.Shift.StartTime);

            // Cho phép trễ tối đa 15 phút (Grace period)
            DateTime lateThreshold = shiftStartTime.AddMinutes(15);

 
            if (nowVn < shiftStartTime.AddMinutes(-30))
                return "It is too early to check in for this shift.";

            if (nowVn > lateThreshold)
            {
                assignment.Status = "Late";
            }
            else
            {
                assignment.Status = "Present";
            }

            assignment.CheckInTime = DateTime.UtcNow;

            var result = await _context.SaveChangesAsync();
            return result > 0 ? null : "Failed to save check-in.";
        }

        public async Task<bool> ApproveCompensationAsync(int assignmentId)
        {
            var assignment = await _context.EmployeeShifts.FindAsync(assignmentId);

            if (assignment == null) return false;

            if (assignment.Status != "Absent") return false;

            assignment.Status = "Compensated";
            assignment.IsPaid = true;

            return await _context.SaveChangesAsync() > 0;
        }
    }
}