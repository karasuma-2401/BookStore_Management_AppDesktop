using BookStoreManagement.API.Models.DTOs;
using BookStoreManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreManagement.API.Controllers
{
    [ApiController]
    [Route("debt-report")]
    [Authorize(Roles = "admin")]
    public class DebtReportController : ControllerBase
    {
        private readonly IDebtReportService _service;

        public DebtReportController(IDebtReportService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDebtReportDTO dto)
        {
            var result = await _service.CreateReport(dto.Month, dto.Year, dto.CustomerId);
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

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var result = await _service.GetByCustomer(customerId);
            return Ok(result);
        }
    }
}