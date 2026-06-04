
using BookStoreManagement.API.Models.Auth;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IUserService
    {

        Task<IEnumerable<UserResponseModel>> GetAllUsersAsync();
        Task<UserResponseModel?> GetUserByIdAsync(int id);
        Task<bool> CreateUserAsync(UserCreateDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<string?> ChangePasswordAsync(int userId, ChangePasswordRequestDto dto);
        Task<string?> AdminChangeStaffPasswordAsync(int employeeId, AdminResetPasswordDto dto);
    }
}
