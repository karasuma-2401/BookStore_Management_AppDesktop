using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Invoice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStoreManagement.API.Controllers
{
    [Route("invoice")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceCreateDto dto)
        {
            try
            {
                var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(claimUserId, out int realUserId))
                {
                    var invoiceId = await _invoiceService.CreateInvoiceAsync(realUserId, dto);

                    if (invoiceId != null)
                    {
                        return Ok(new
                        {
                            Success = true,
                            Message = "Checkout successful!",
                            InvoiceId = invoiceId
                        });
                    }

                    return BadRequest(new { Success = false, Message = "Failed to create invoice." });
                }
                else
                {
                    return Unauthorized(new { Success = false, Message = "Invalid Token. Cannot identify user." });
                }
            }
            catch (Exception ex)
            {
                var realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { Success = false, Message = realError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return Ok(invoices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null) return NotFound(new { Message = "Invoice not found." });

            return Ok(invoice);
        }

        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelInvoice(int id)
        {

            var result = await _invoiceService.CancelInvoiceAsync(id);

            if (!result)
                return BadRequest(new { Success = false, Message = "Invoice not found or already canceled." });

            return Ok(new { Success = true, Message = "Invoice canceled successfully. Stock restored." });
        }
    }
}
