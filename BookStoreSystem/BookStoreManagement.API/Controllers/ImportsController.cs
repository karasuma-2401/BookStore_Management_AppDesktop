using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using BookStoreManagement.API.Interfaces.Services;
using Microsoft.AspNetCore.SignalR; 
using BookStoreManagement.API.Hubs;   

namespace BookStoreManagement.API.Controllers
{
    [Route("import")]
    [Authorize]
    [ApiController]
    public class ImportsController : ControllerBase
    {
        private readonly IImportService _service;
        private readonly IHubContext<BookHub, IBookHubClient> _hubContext;

        public ImportsController(IImportService service, IHubContext<BookHub, IBookHubClient> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ImportCreateDto dto)
        {
            // Log incoming claims for debugging
            System.Diagnostics.Debug.WriteLine("[ImportsController] Incoming claims:");
            foreach (var c in User.Claims)
            {
                System.Diagnostics.Debug.WriteLine($" - {c.Type} = {c.Value}");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.Name);
            if (userIdClaim == null)
            {
                return Unauthorized(new { Success = false, Message = "Invalid Token. Cannot identify user." });
            }

            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { Success = false, Message = "Invalid Token. Cannot identify user." });
            }

            var result = await _service.CreateImport(dto, userId);

            if (result != null && dto.Details != null)
            {
                foreach (var detail in dto.Details)
                {
                    await _hubContext.Clients.All.InventoryStockChanged(detail.BookId, detail.Quantity);
                }

                await _hubContext.Clients.All.ImportCreated();
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetImports();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetImportById(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}