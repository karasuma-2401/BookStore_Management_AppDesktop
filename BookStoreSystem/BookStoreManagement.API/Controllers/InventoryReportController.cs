using BookStoreManagement.API.Models.DTOs;
using BookStoreManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreManagement.API.Controllers
{
    [ApiController]
    [Route("inventory-report")]
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
    }
}