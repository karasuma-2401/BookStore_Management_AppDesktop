using BookStoreManagement.API.Models.DTOs;
using BookStoreManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreManagement.API.Controllers
{
    [Route("setting")]
    [ApiController]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingService _service;

        public SettingsController(ISettingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAll();
            return Ok(result);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await _service.GetByName(name);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromQuery] string name, [FromQuery] string value)
        {
            var result = await _service.Create(name, value);
            return Ok(result);
        }

        [HttpPut("{name}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(string name, [FromBody] UpdateSettingDTO dto)
        {
            var result = await _service.Update(name, dto.Value);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{name}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string name)
        {
            var success = await _service.Delete(name);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}