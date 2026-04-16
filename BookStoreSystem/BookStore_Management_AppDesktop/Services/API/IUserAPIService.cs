using BookStore_Management_AppDesktop.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API
{
    public interface IUserApiService
    {
        Task<List<UserResponseModel>> GetAllUsersAsync();
        Task<(bool IsSuccess, string Message)> CreateUserAsync(UserCreateDto dto);
    }
}