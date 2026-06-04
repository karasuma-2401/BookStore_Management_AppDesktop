using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStoreManagement.API.Controllers
{
    [Route("payment")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment([FromBody] PaymentCreateDto dto)
        {
            try
            {
                var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(claimUserId, out int realUserId))
                {
                    var result = await _paymentService.CreatePaymentAsync(realUserId, dto);

                    if (result)
                    {
                        return Ok(new { Success = true, Message = "Payment completed successfully!" });
                    }

                    return BadRequest(new { Success = false, Message = "Payment failed." });
                }

                return Unauthorized(new { Success = false, Message = "Invalid Token." });
            }
            catch (Exception ex)
            {
                var realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { Success = false, Message = realError });
            }
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetPaymentsByInvoiceId(int invoiceId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByInvoiceIdAsync(invoiceId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelPayment(int id)
        {
            try
            {
                var result = await _paymentService.CancelPaymentAsync(id);

                if (result)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Payment cancelled successfully. Outstanding balance has been restored."
                    });
                }

                return BadRequest(new
                {
                    Success = false,
                    Message = "Payment not found."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}