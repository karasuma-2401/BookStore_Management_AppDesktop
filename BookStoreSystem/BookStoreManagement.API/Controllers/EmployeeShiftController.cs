using Microsoft.AspNetCore.Authorization;
using BookStoreManagement.API.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookStoreManagement.API.Models.Shift;

namespace BookStoreManagement.API.Controllers
{
    [Route("employeeshift")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class EmployeeShiftController : ControllerBase
    {
        private readonly IEmployeeShift _employeeShiftService;

        public EmployeeShiftController(IEmployeeShift employeeShiftService)
        {
            _employeeShiftService = employeeShiftService;
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignShift([FromBody] ShiftAssignDto dto)
        {
            var errorMessage = await _employeeShiftService.AssignShiftAsync(dto);
            if (errorMessage != null)
                return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Shift assigned successfully!" });
        }

        [HttpGet("schedule")]
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
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var success = await _employeeShiftService.DeleteAssignmentAsync(id);
            if (!success)
                return NotFound(new { message = "Assignment not found." });
            return Ok(new { message = "Assignment deleted successfully!" });

        }
    }
}
