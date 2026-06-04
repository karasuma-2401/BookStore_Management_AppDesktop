using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStoreManagement.API.Controllers
{
    [Route("user")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: user
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: user/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null
            ? NotFound(new { message = "User not found." })
            : Ok(user);
        }

        // POST: user
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PostUser(UserCreateDto user)
        {
            try
            {
                var success = await _userService.CreateUserAsync(user);

                return success
                    ? StatusCode(201, new { message = "User created successfully" })
                    : BadRequest(new { message = "Invalid data" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: user/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);

            return success
                ? NoContent()
                : NotFound(new { message = "User not found for deletion." });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var errorMessage = await _userService.ChangePasswordAsync(userId, dto);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(new { message = "Password updated successfully!" });
        }

        // POST: user/{id}/admin-change-password
        [HttpPost("{id}/admin-change-password")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminChangeStaffPassword(int id, [FromBody] AdminResetPasswordDto dto)
        {
            var errorMessage = await _userService.AdminChangeStaffPasswordAsync(id, dto);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }
            return Ok(new { message = "\"Employee password changed successfully." });

        }
    }
}