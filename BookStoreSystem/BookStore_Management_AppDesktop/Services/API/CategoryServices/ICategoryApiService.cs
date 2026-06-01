using BookStore_Management_AppDesktop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.CategoryServices
{
    public interface ICategoryApiService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> CreateCategoryAsync(string name);
    }
}