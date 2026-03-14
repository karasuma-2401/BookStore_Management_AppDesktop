using BookStoreManagement.API.Data;
using BookStoreManagement.API.Handlers;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Auth;
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
        private readonly IValidator<ResetPasswordRequestDto> _resetPasswordValidator;

        public UsersController(IUserService userService, IValidator<ResetPasswordRequestDto> resetPasswordValidator)
        {
            _userService = userService;
            _resetPasswordValidator = resetPasswordValidator;
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
            ? NotFound(new { message = "User not found." })
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
                : BadRequest(new { message = "Update failed or ID does not match." });
        }

        // POST: user
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostUser(User user)
        {
            var success = await _userService.CreateUserAsync(user);

            return success
                ? CreatedAtAction("GetUser", new { id = user.UserId }, new { message = "User created successfully" })
                : BadRequest(new { message = "Invalid data" });
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

        // POST: user/forgot-password
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            var success = await _userService.SendForgotPasswordEmailAsync(dto.Email);

            return success
                ? Ok(new { message = "The verification code has been sent to your email." })
                : NotFound(new { message = "This email is not registered in the system." });
        }

        // POST: user/reset-password
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto dto)
        {
            var validatorResult = await _resetPasswordValidator.ValidateAsync(dto);

            if (!validatorResult.IsValid) { 
                var errors = validatorResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(new { message = "Invalid data", errors = errors });
            }

            var success = await _userService.ResetPasswordAsync(dto.Token, dto.NewPassword, dto.ConfirmPassword);

            return success
                ? Ok(new { message = "Password changed successfully" })
                : BadRequest(new { message = "The verification code is incorrect or has expired." });
        }
    }
}
