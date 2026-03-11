using BookStore_Management_AppDesktop.Models;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services
{
    public interface IAuthService
    {
        Task<LoginResponseModel?> LoginAsync(string username, string password);
    }
}