using BookStoreManagement.API.Data;
using BookStoreManagement.API.Handlers;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Auth;
using BookStoreManagement.API.Models.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDBContext _context;
        private readonly IValidator<UserCreateDto> _createValidator;
        private readonly IValidator<AdminResetPasswordDto> _adminResetPassValidator;
        public UserService(
            ApplicationDBContext context,
            IValidator<UserCreateDto> createValidator,
            IValidator<AdminResetPasswordDto> adminResetPassValidator)
        {
            _context = context;
            _createValidator = createValidator;
            _adminResetPassValidator = adminResetPassValidator;
        }

        public async Task<IEnumerable<UserResponseModel>> GetAllUsersAsync()
        {
            return await _context.Users
                .Where(u => u.Status == 1)
                .Select(u => new UserResponseModel
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    RoleId = u.RoleId,
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
                RoleId = user.RoleId,
                Status = user.Status
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

        public async Task<string?> AdminChangeStaffPasswordAsync(int employeeId, AdminResetPasswordDto dto)
        {

            var validationResult = await _adminResetPassValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return validationResult.Errors.First().ErrorMessage;
            }

            var user = await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Employee != null && u.Employee.EmployeeId == employeeId);

            if (user == null) return "Employee account not found.";

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return null;
        }

        public async Task<bool> CreateUserAsync(UserCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return false;
            }

            var isUsernameExist = await _context.Users.AnyAsync(u => u.Username == dto.Username);
            if (isUsernameExist)
            {
                throw new Exception("This Username already exists in the system!");
            }

            var user = new User
            {
                Username = dto.Username,
                RoleId = dto.RoleId.ToString(),
                PasswordHash = PasswordHashHandler.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                Status = 1
            };

            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;
            user.Status = 0;

            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}