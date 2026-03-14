using BookStoreManagement.API.Data;
using BookStoreManagement.API.Handlers;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.DTOs;
using BookStoreManagement.API.Models.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null
            ? NotFound(new { message = "Khong tim thay nguoi dung" })
            : Ok(user);
        }

        // PUT: user/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            var success = await _userService.UpdateUserAsync(id, user);

            return success
                ? NoContent()
                : BadRequest(new { message = "Cap nhat that bai hoac ID khong khop" });
        }

        // POST: user
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostUser(User user)
        {
            var success = await _userService.CreateUserAsync(user);

            return success
                ? CreatedAtAction("GetUser", new { id = user.UserId }, new { message = "Tao user thanh cong" })
                : BadRequest(new { message = "Du lieu khong hop le" });
        }

        // DELETE: user/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);

            return success
                ? NoContent()
                : NotFound(new { message = "Khong tim thay user de xoa" });
        }

        // POST: user/forgot-password
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var success = await _userService.SendForgotPasswordEmailAsync(email);

            return success
                ? Ok(new { message = "Ma xac thuc da duoc gui vao email cua ban" })
                : NotFound(new { message = "Email nay khong ton tai trong he thong" });
        }

        // POST: user/reset-password
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string token, string newPassword)
        {
            var success = await _userService.ResetPasswordAsync(token, newPassword);

            return success
                ? Ok(new { message = "Doi mat khau thanh cong" })
                : BadRequest(new { message = "Ma xac thuc sai hoac da het han" });
        }
    }
}
