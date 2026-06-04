using BookStoreManagement.API.Models.DTOs;
using BookStoreManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreManagement.API.Controllers
{
    [ApiController]
    [Route("inventory-report")]
    [Authorize(Roles = "admin")]
    public class InventoryReportController : ControllerBase
    {
        private readonly IInventoryReportService _service;

        public InventoryReportController(IInventoryReportService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInventoryReportDTO dto)
        {
            var result = await _service.CreateReport(dto.Month, dto.Year, dto.BookId);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetReports([FromQuery] int month, [FromQuery] int year)
        {
            var result = await _service.GetReports(month, year);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetByBook(int bookId)
        {
            var result = await _service.GetByBook(bookId);
            return Ok(result);
        }
    }
}