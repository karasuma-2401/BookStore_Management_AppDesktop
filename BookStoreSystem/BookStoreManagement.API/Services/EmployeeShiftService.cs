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

        private DateTime GetWorkDateVn(DateTime workDate)
        {
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var utcDate = DateTime.SpecifyKind(workDate, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, vnTimeZone).Date;
        }

        public async Task<string?> AssignShiftAsync(ShiftAssignDto dto)
        {
            if (!await _context.Shifts.AnyAsync(s => s.ShiftId == dto.ShiftId))
                return "Shift not found.";

            if (!await _context.Employees.AnyAsync(e => e.EmployeeId == dto.EmployeeId))
                return "Employee not found.";

            // Validation: Cannot schedule a shift for a date or month in the past
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime nowVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
            var todayVn = nowVn.Date;

            // Chuyển đổi dto.WorkDate sang múi giờ Việt Nam trước khi lấy phần ngày để tránh lệch ngày
            var workDateUtcInput = DateTime.SpecifyKind(dto.WorkDate, DateTimeKind.Utc);
            var workDateVn = TimeZoneInfo.ConvertTimeFromUtc(workDateUtcInput, vnTimeZone).Date;

            if (workDateVn < todayVn)
                return "Cannot schedule a shift for a date or month in the past.";

            // Đảm bảo kiểu múi giờ là UTC và chỉ lấy phần Ngày (Date) cho PostgreSQL timestamptz
            var workDateUtc = DateTime.SpecifyKind(workDateVn, DateTimeKind.Utc);

            if (await _context.EmployeeShifts.AnyAsync(es => es.EmployeeId == dto.EmployeeId && es.WorkDate == workDateUtc && es.ShiftId == dto.ShiftId))
                return "Employee already has this shift assigned for this date.";

            var employeeshift = new EmployeeShift
            {
                EmployeeId = dto.EmployeeId,
                ShiftId = dto.ShiftId,
                WorkDate = workDateUtc,
                Status = "Scheduled",
                CheckInTime = null,
                IsPaid = false
            };

            _context.EmployeeShifts.Add(employeeshift);
            var result = await _context.SaveChangesAsync();
            return result > 0 ? null : "Could not save the assignment to database.";
        }

        public async Task<IEnumerable<EmployeeShiftResponseDto>> GetScheduleAsync(DateTime startDate, DateTime endDate, int? employeeId = null)
        {
            var startUtc = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
            var endUtc = DateTime.SpecifyKind(endDate.Date.AddDays(1), DateTimeKind.Utc);

            IQueryable<EmployeeShift> query = _context.EmployeeShifts
                .Include(es => es.Employee)
                .Include(es => es.Shift)
                // Sử dụng khoảng ngày bao phủ toàn bộ ngày cuối cùng
                .Where(es => es.WorkDate >= startUtc && es.WorkDate < endUtc);

            if (employeeId.HasValue)
            {
                query = query.Where(es => es.EmployeeId == employeeId.Value);
            }

            var list = await query.ToListAsync();

            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime nowVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);

            bool changed = false;
            foreach (var es in list)
            {
                if (es.Status == "Scheduled" && es.Shift != null)
                {
                    var shiftEndTime = GetWorkDateVn(es.WorkDate).Add(es.Shift.EndTime);
                    if (nowVn > shiftEndTime)
                    {
                        es.Status = "Absent";
                        es.IsPaid = false;
                        changed = true;
                    }
                }
            }
            if (changed)
            {
                await _context.SaveChangesAsync();
            }

            return list.Select(es => new EmployeeShiftResponseDto
            {
                Id = es.Id,
                FullName = es.Employee?.FullName ?? string.Empty,
                ShiftName = es.Shift?.ShiftName ?? string.Empty,
                WorkTime = es.Shift != null ? $"{es.Shift.StartTime} - {es.Shift.EndTime}" : string.Empty,
                WorkDate = GetWorkDateVn(es.WorkDate).ToString("yyyy-MM-dd"),
                Status = es.Status,
                CheckInTime = es.CheckInTime,
                IsPaid = es.IsPaid
            }).ToList();
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


            DateTime shiftStartTime = GetWorkDateVn(assignment.WorkDate).Add(assignment.Shift.StartTime);

            // Cho phép trễ tối đa 15 phút (Grace period)
            DateTime lateThreshold = shiftStartTime.AddMinutes(15);

 
            if (nowVn < shiftStartTime.AddMinutes(-30))
                return "It is too early to check in for this shift.";

            if (nowVn <= shiftStartTime)
            {
                assignment.Status = "Present";
                assignment.IsPaid = true;
            }
            else if (nowVn <= lateThreshold)
            {
                assignment.Status = "Late";
                assignment.IsPaid = true;
            }
            else
            {
                assignment.Status = "Absent";
                assignment.IsPaid = false;
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

        public async Task<PayslipDto?> CalculateSalaryAsync(int employeeId, int month, int year)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return null;

            // Xác định khoảng thời gian đầu tháng và đầu tháng tiếp theo ở dạng UTC
            var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endDate = startDate.AddMonths(1);

            var shiftsInMonth = await _context.EmployeeShifts
                .Include(es => es.Shift)
                .Where(es => es.EmployeeId == employeeId
                          && es.WorkDate >= startDate
                          && es.WorkDate < endDate)
                .ToListAsync();

            int totalAssigned = shiftsInMonth.Count;

            var payslip = new PayslipDto
            {
                EmployeeId = employeeId,
                FullName = employee.FullName,
                Month = month,
                Year = year,
                Salary = employee.Salary,
                TotalAssignedShifts = totalAssigned,
                WorkedShifts = 0,
                AbsentShifts = 0,
                ActualSalary = 0
            };

            if (totalAssigned == 0)
            {
                return payslip;
            }

            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime nowVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);

            int workedCount = 0;
            int absentCount = 0;

            foreach (var es in shiftsInMonth)
            {
                if (es.Status == "Present" || es.Status == "Late" || es.Status == "Compensated")
                {
                    workedCount++;
                }
                else if (es.Status == "Absent")
                {
                    absentCount++;
                }
                else if (es.Status == "Scheduled")
                {
                    // Nếu thời điểm hiện tại đã vượt quá giờ kết thúc ca làm việc,
                    // mà nhân viên chưa check-in (vẫn ở trạng thái Scheduled) thì tính là Absent
                    var shiftEndTime = GetWorkDateVn(es.WorkDate).Add(es.Shift?.EndTime ?? TimeSpan.Zero);
                    if (nowVn > shiftEndTime)
                    {
                        absentCount++;
                    }
                }
            }

            payslip.WorkedShifts = workedCount;
            payslip.AbsentShifts = absentCount;

            // Tính lương trực tiếp theo ca: Lương thực nhận = Đơn giá mỗi ca * Số ca thực làm - 20% phạt ca vắng mặt
            var baseSalary = payslip.Salary * payslip.WorkedShifts;
            var fine = payslip.Salary * 0.20m * payslip.AbsentShifts;
            payslip.ActualSalary = Math.Max(0, Math.Round(baseSalary - fine, 0));

            return payslip;
        }

        public async Task<ShiftDayDetailResponseDto> GetDayDetailAsync(DateTime date)
        {
            var dateUtc = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var nextDayUtc = dateUtc.AddDays(1);

            var shifts = await _context.Shifts.OrderBy(s => s.StartTime).ToListAsync();
            var assignments = await _context.EmployeeShifts
                .Include(es => es.Employee)
                .Include(es => es.Shift)
                .Where(es => es.WorkDate >= dateUtc && es.WorkDate < nextDayUtc)
                .ToListAsync();

            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime nowVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);

            bool changed = false;
            foreach (var es in assignments)
            {
                if (es.Status == "Scheduled" && es.Shift != null)
                {
                    var shiftEndTime = GetWorkDateVn(es.WorkDate).Add(es.Shift.EndTime);
                    if (nowVn > shiftEndTime)
                    {
                        es.Status = "Absent";
                        es.IsPaid = false;
                        changed = true;
                    }
                }
            }
            if (changed)
            {
                await _context.SaveChangesAsync();
            }

            var response = new ShiftDayDetailResponseDto
            {
                Date = date.Date,
                Shifts = new List<ShiftDayItemDto>()
            };

            foreach (var shift in shifts)
            {
                var assignment = assignments.FirstOrDefault(a => a.ShiftId == shift.ShiftId);
                var item = new ShiftDayItemDto
                {
                    ShiftId = shift.ShiftId,
                    ShiftName = shift.ShiftName,
                    WorkTime = $"{shift.StartTime:hh\\:mm} - {shift.EndTime:hh\\:mm}",
                    Status = "Empty"
                };

                if (assignment != null)
                {
                    item.AssignmentId = assignment.Id;
                    item.EmployeeId = assignment.EmployeeId;
                    item.FullName = assignment.Employee.FullName;
                    item.Status = assignment.Status;
                    item.CheckInTime = assignment.CheckInTime;
                    item.IsPaid = assignment.IsPaid;
                }

                response.Shifts.Add(item);
            }

            return response;
        }

        public async Task<KioskCheckInResponseDto> KioskCheckInAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return new KioskCheckInResponseDto { Success = false, Message = "Employee not found." };
            }

            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime nowVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
            var todayVn = nowVn.Date;

            var assignments = await _context.EmployeeShifts
                .Include(es => es.Employee)
                .Include(es => es.Shift)
                .Where(es => es.EmployeeId == employeeId)
                .ToListAsync();

            var todayAssignments = assignments
                .Where(es => GetWorkDateVn(es.WorkDate) == todayVn)
                .OrderBy(es => es.Shift?.StartTime)
                .ToList();

            if (!todayAssignments.Any())
            {
                return new KioskCheckInResponseDto 
                { 
                    Success = false, 
                    Message = $"No shift scheduled for today ({nowVn:dd/MM/yyyy}) for {employee.FullName}." 
                };
            }

            var assignment = todayAssignments.FirstOrDefault(es => es.Status == "Scheduled");
            if (assignment == null)
            {
                assignment = todayAssignments.Last();
            }

            if (assignment.Status == "Present" || assignment.Status == "Late")
            {
                return new KioskCheckInResponseDto
                {
                    Success = false,
                    Message = "You have already checked in for this shift.",
                    EmployeeName = employee.FullName,
                    ShiftName = assignment.Shift?.ShiftName ?? string.Empty,
                    WorkTime = assignment.Shift != null ? $"{assignment.Shift.StartTime:hh\\:mm} - {assignment.Shift.EndTime:hh\\:mm}" : string.Empty,
                    CheckInTime = assignment.CheckInTime,
                    Status = assignment.Status
                };
            }

            if (assignment.Status != "Scheduled")
            {
                return new KioskCheckInResponseDto
                {
                    Success = false,
                    Message = $"Cannot check in. Current shift status is {assignment.Status}.",
                    EmployeeName = employee.FullName
                };
            }

            DateTime shiftStartTime = GetWorkDateVn(assignment.WorkDate).Add(assignment.Shift?.StartTime ?? TimeSpan.Zero);
            DateTime lateThreshold = shiftStartTime.AddMinutes(15);

            if (nowVn < shiftStartTime.AddMinutes(-30))
            {
                return new KioskCheckInResponseDto
                {
                    Success = false,
                    Message = "It is too early to check in for this shift. Please wait until 30 minutes before starting time.",
                    EmployeeName = employee.FullName,
                    ShiftName = assignment.Shift?.ShiftName ?? string.Empty,
                    WorkTime = assignment.Shift != null ? $"{assignment.Shift.StartTime:hh\\:mm} - {assignment.Shift.EndTime:hh\\:mm}" : string.Empty,
                    Status = "Scheduled"
                };
            }

            string newStatus = "Present";
            bool isPaid = true;
            string message = "Check-in successful! Welcome to work.";
            bool success = true;

            if (nowVn > shiftStartTime && nowVn <= lateThreshold)
            {
                newStatus = "Late";
                message = "Check-in successful! (You arrived late)";
            }
            else if (nowVn > lateThreshold)
            {
                newStatus = "Absent";
                isPaid = false;
                success = false;
                message = "You are more than 15 minutes late. You have been marked as Absent.";
            }

            assignment.Status = newStatus;
            assignment.IsPaid = isPaid;
            assignment.CheckInTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new KioskCheckInResponseDto
            {
                Success = success,
                Message = message,
                EmployeeName = employee.FullName,
                ShiftName = assignment.Shift?.ShiftName ?? string.Empty,
                WorkTime = assignment.Shift != null ? $"{assignment.Shift.StartTime:hh\\:mm} - {assignment.Shift.EndTime:hh\\:mm}" : string.Empty,
                CheckInTime = assignment.CheckInTime,
                Status = newStatus
            };
        }
    }
}