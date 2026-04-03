using BookStore_Management_AppDesktop.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API
{
    public interface IUserApiService
    {
        // Lấy danh sách User để đổ vào ComboBox
        Task<List<UserResponseModel>> GetAllUsersAsync();
    }
}