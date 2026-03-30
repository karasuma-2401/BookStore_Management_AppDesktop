using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Shift;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreManagement.API.Controllers
{
    [Route("shift")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class ShiftsController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftsController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShifts()
        {
            var shifts = await _shiftService.GetAllShiftsAsync();
            return Ok(shifts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShift(int id)
        {
            var shift = await _shiftService.GetShiftByIdAsync(id);
            if (shift == null)
                return NotFound(new { message = "Shift not found." });

            return Ok(shift);
        }

        [HttpPut("{id}/time")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateShiftTime(int id, [FromBody] ShiftTimeUpdateDto dto)
        {

            var errorMessage = await _shiftService.UpdateShiftTimeAsync(id, dto);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(new { message = "Shift times updated successfully!" });
        }
    }


}

