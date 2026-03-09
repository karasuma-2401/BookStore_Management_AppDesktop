using BookStoreManagement.API.Data;
using BookStoreManagement.API.Handlers;
using BookStoreManagement.API.Models.DTOs;
using BookStoreManagement.API.Models.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreManagement.API.Controllers
{
    [Route("user")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IValidator<User> _validator;

        public UsersController(ApplicationDBContext context, IValidator<User> validator)
        {
            _context = context;
            _validator = validator;
        }
        public UsersController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<UserResponseModel>>> GetUsers()
        {
            return await _context.Users
            .Select(u => new UserResponseModel
            {
                UserId = u.UserId,
                Username = u.Username,
                FullName = u.FullName,
                RoleId = u.RoleId
            })
            .ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UserResponseModel>> GetUser(int id)
        {
            // Tìm user trong Database
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                // Trả về 404 nếu không tìm thấy, kèm thông báo không dấu
                return NotFound(new { message = "Khong tim thay nguoi dung" });
            }

            // Ánh xạ sang DTO để giấu PasswordHash
            var userDto = new UserResponseModel
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                RoleId = user.RoleId
            };

            return userDto;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest(new { message = "ID khong khop" });
            }

            // 1. Chạy Validation thiết kế
            var validationResult = await _validator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            // 2. Kiểm tra xem User có tồn tại không
            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
            if (existingUser == null)
            {
                return NotFound(new { message = "Khong tim thay user de cap nhat" });
            }

            // 3. Xử lý mật khẩu: 
            // Nếu mật khẩu gửi lên khác với mật khẩu cũ trong DB -> Nghĩa là họ muốn đổi pass -> Cần băm mới
            if (user.PasswordHash != existingUser.PasswordHash)
            {
                user.PasswordHash = PasswordHashHandler.HashPassword(user.PasswordHash);
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id)) return NotFound();
                else throw;
            }

            return NoContent(); // Trả về 204 nếu thành công
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> PostUser(User user, [FromServices] IValidator<User> validator)
        {
            // 1. Thực hiện kiểm tra lỗi
            var validationResult = await validator.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                // Trả về lỗi 400 và danh sách các lỗi không dấu
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            // BAM MAT KHAU truoc khi luu vao DB
            user.PasswordHash = PasswordHashHandler.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, new { message = "Tao user thanh cong" });
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Khong tim thay user" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
