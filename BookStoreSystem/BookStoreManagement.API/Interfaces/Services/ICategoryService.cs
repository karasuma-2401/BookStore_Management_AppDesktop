using BookStoreManagement.API.DTOs.Categories;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAll();
        Task<bool> Update(int id, Category category); 
        Task<Category?> GetById(int id);
        Task<Category> Create(Category category);
        Task<bool> Delete(int id);
    }
}