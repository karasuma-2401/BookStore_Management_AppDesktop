using BookStoreManagement.API.Models.DTOs;
using BookStoreManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreManagement.API.Controllers
{
    [ApiController]
    [Route("debt-report")]
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
    }
}