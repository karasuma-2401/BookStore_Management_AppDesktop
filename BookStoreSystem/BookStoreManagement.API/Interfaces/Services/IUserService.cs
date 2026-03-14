using BookStoreManagement.API.Models.DTOs;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IUserService
    {

        Task<IEnumerable<UserResponseModel>> GetAllUsersAsync();
        Task<UserResponseModel?> GetUserByIdAsync(int id);
        Task<bool> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(int id, User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> SendForgotPasswordEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);


    }
}
