
using BookStoreManagement.API.Models.Auth;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IUserService
    {

        Task<IEnumerable<UserResponseModel>> GetAllUsersAsync();
        Task<UserResponseModel?> GetUserByIdAsync(int id);
        Task<bool> CreateUserAsync(UserCreateDto dto);
        Task<bool> UpdateUserAsync(int id, UserUpdateDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> SendForgotPasswordEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword, string confirmPassword);
        Task<string?> ChangePasswordAsync(int userId, ChangePasswordRequestDto dto);
    }
}
