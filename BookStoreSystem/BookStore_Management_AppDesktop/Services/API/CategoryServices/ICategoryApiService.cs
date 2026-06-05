using BookStore_Management_AppDesktop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.CategoryServices
{
    public interface ICategoryApiService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> CreateCategoryAsync(Category newCategory);
        Task<bool> UpdateCategoryAsync(int id, string name); 
        Task<bool> DeleteCategoryAsync(int id);
    }
}