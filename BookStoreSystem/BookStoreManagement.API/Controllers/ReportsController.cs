using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreManagement.API.Controllers
{
    [Route("report")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: report/monthly?month=5&year=2026
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyReport(
            [FromQuery] int month,
            [FromQuery] int year)
        {
            if (month < 1 || month > 12)
                return BadRequest("Month must be between 1 and 12.");

            if (year < 2000 || year > DateTime.Now.Year + 1)
                return BadRequest("Year is not valid.");

            var result = await _reportService.GetMonthlyReport(month, year);

            return Ok(result);
        }
    }
}