using BookStore_Management_AppDesktop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.AuthorServices
{
    public interface IAuthorApiService
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<Author?> CreateAuthorAsync(string name);
        Task<bool> UpdateAuthorAsync(int id, string name);
        Task<bool> DeleteAuthorAsync(int id);
    }
}
