using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Shift;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStoreManagement.API.Controllers
{
    [Route("employeeshift")]
    [ApiController]
    [Authorize]
    public class EmployeeShiftController : ControllerBase
    {
        private readonly IEmployeeShift _employeeShiftService;

        public EmployeeShiftController(IEmployeeShift employeeShiftService)
        {
            _employeeShiftService = employeeShiftService;
        }
        [Authorize(Roles = "admin")]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignShift([FromBody] ShiftAssignDto dto)
        {
            var errorMessage = await _employeeShiftService.AssignShiftAsync(dto);
            if (errorMessage != null)
                return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Shift assigned successfully!" });
        }

        [HttpGet("schedule")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetSchedule([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? employeeId)
        {
            if (startDate > endDate)
            {
                return BadRequest(new { message = "Start date must be before end date." });
            }
            var schedule = await _employeeShiftService.GetScheduleAsync(startDate, endDate,employeeId);
            return Ok(schedule);
        }

        [HttpDelete("assignment/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var success = await _employeeShiftService.DeleteAssignmentAsync(id);
            if (!success)
                return NotFound(new { message = "Assignment not found." });
            return Ok(new { message = "Assignment deleted successfully!" });

        }

        [HttpPut("checkin/{id}")]
        public async Task<IActionResult> CheckIn(int id)
        {

            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new { message = "Invalid token." });

            int currentUserId = int.Parse(userIdClaim.Value);

            var errorMessage = await _employeeShiftService.CheckInAsync(id, currentUserId);

            if (errorMessage != null)
                return BadRequest(new { message = errorMessage });

            return Ok(new { message = "Checked in successfully!" });
        }

        [HttpPut("compensate/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ApproveCompensation(int id)
        {
            var success = await _employeeShiftService.ApproveCompensationAsync(id);
            if (!success)
                return BadRequest(new { message = "Failed to approve compensation. Make sure the shift is marked as Absent." });

            return Ok(new { message = "Compensation approved successfully!" });
        }

        [HttpGet("payroll/{employeeId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetPayroll(int employeeId, [FromQuery] int month, [FromQuery] int year)
        {
            if (month < 1 || month > 12 || year < 2000)
                return BadRequest(new { message = "Invalid month or year." });

            var payslip = await _employeeShiftService.CalculateSalaryAsync(employeeId, month, year);

            if (payslip == null)
                return NotFound(new { message = "Employee not found." });

            return Ok(payslip);
        }

        [HttpGet("day-detail")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetDayDetail([FromQuery] DateTime date)
        {
            var dayDetail = await _employeeShiftService.GetDayDetailAsync(date);
            return Ok(dayDetail);
        }

        [HttpPost("kiosk-checkin")]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> KioskCheckIn([FromBody] KioskCheckInRequestDto dto)
        {
            if (dto == null || dto.EmployeeId <= 0)
                return BadRequest(new { message = "Invalid Employee ID." });

            var result = await _employeeShiftService.KioskCheckInAsync(dto.EmployeeId);
            return Ok(result);
        }
    }
}
