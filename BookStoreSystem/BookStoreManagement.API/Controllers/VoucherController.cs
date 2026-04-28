using BookStoreManagement.API.Data;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Models.Voucher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Controllers
{
    [Route("voucher")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vouchers = await _voucherService.GetAllAsync();
            return Ok(vouchers);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VoucherCreateDto dto)
        {
            try
            {
                var voucher = await _voucherService.CreateAsync(dto);
                return Ok(new
                {
                    Message = "Voucher created successfully.",
                    Data = voucher
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _voucherService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new { Message = "Voucher not found." });
            }

            return Ok(new { Message = "Voucher deleted successfully." });
        }

    }
}
