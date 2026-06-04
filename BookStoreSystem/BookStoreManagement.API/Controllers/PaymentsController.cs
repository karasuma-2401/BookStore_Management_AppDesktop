using BookStoreManagement.API.Data;
using BookStoreManagement.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStoreManagement.API.Controllers
{
    [Route("payment")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public PaymentsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invoice = await _context.Invoices.FindAsync(dto.InvoiceId);
                if (invoice == null)
                    return NotFound(new { message = "Invoice not found." });

                if (invoice.CustomerId == null)
                    return BadRequest(new { message = "Cannot add payment for an invoice checked out as Guest." });

                var customer = await _context.Customers.FindAsync(invoice.CustomerId);
                if (customer == null)
                    return NotFound(new { message = "Customer not found." });

                var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int? realUserId = null;
                if (int.TryParse(claimUserId, out int uid))
                    realUserId = uid;

                var payment = new Payment
                {
                    CustomerId = invoice.CustomerId,
                    InvoiceId = dto.InvoiceId,
                    UserId = realUserId,
                    Amount = dto.Amount,
                    PaymentDate = DateTime.UtcNow
                };

                _context.Payments.Add(payment);

                // Reduce customer's debt
                customer.Debt = Math.Max(0, customer.Debt - dto.Amount);
                _context.Customers.Update(customer);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { success = true, message = "Payment recorded successfully, customer debt updated." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { message = "Payment Error: " + ex.Message });
            }
        }
    }

    public class PaymentCreateDto
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
    }
}
