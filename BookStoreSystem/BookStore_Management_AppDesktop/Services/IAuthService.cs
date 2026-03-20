using BookStore_Management_AppDesktop.Models.Dtos;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services
{
    public interface IAuthService
    {
        Task<LoginResponseModel?> LoginAsync(string username, string password);
        Task<bool> ForgotPasswordAsync (string email);
        Task<bool> ResetPasswordAsync (string token, string newPassword, string confirmPassword);
    }
}