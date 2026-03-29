using BookStoreManagement.API.Data;
using BookStoreManagement.API.Handlers;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Interfaces.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using BookStoreManagement.API.Models.Auth;

namespace BookStoreManagement.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDBContext _context;
        private readonly IValidator<User> _validator;
        private readonly IEmailService _emailService;

        public UserService(ApplicationDBContext context, IValidator<User> validator, IEmailService emailService)
        {
            _context = context;
            _validator = validator;
            _emailService = emailService;
        }

        public async Task<IEnumerable<UserResponseModel>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserResponseModel
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    RoleId = u.RoleId,
                    Email = u.Email
                }).ToListAsync();
        }

        public async Task<UserResponseModel?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;
            return new UserResponseModel
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                RoleId = user.RoleId,
                Email = user.Email
            };
        }

        public async Task<string?> ChangePasswordAsync(int userId, ChangePasswordRequestDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return "User not found.";

            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
                return "Incorrect old password.";

            if (dto.NewPassword != dto.ConfirmPassword)
                return "Confirm password does not match.";

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return null;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            var validationResult = await _validator.ValidateAsync(user);
            if (!validationResult.IsValid) return false;

            user.PasswordHash = PasswordHashHandler.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserAsync(int id, User user)
        {
            if (id != user.UserId) return false;

            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
            if (existingUser == null) return false;


            if (user.PasswordHash != existingUser.PasswordHash)
            {
                user.PasswordHash = PasswordHashHandler.HashPassword(user.PasswordHash);
            }

            _context.Entry(user).State = EntityState.Modified;
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;
            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SendForgotPasswordEmailAsync(string email) 
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return false;

            user.ResetToken = new Random().Next(100000, 999999).ToString();

            user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(5);

            await _context.SaveChangesAsync();

            string subject = "Yêu cầu đặt lại mật khẩu - App Bán Sách";
            string body = $@"
            <h3>Xin chào,</h3>
            <p>Mã xác thực (OTP) để khôi phục mật khẩu của bạn là: <b style='font-size: 24px; color: red;'>{user.ResetToken}</b></p>
            <p>Mã này sẽ hết hạn sau 5 phút. Vui lòng không chia sẻ cho bất kỳ ai.</p>
            <p>Trân trọng,<br>Hệ thống Admin</p>";

            await _emailService.SendEmailAsync(email, subject, body);

            return true;

        }
        public async Task<bool> ResetPasswordAsync(string token, string newPassword, string confirmPassword) 
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.ResetToken == token && u.ResetTokenExpires > DateTime.UtcNow);

            if (user == null) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();

            return true;

        }

    }
}
