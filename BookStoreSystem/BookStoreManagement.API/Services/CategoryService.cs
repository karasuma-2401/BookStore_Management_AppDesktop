using BookStoreManagement.API.Data;
using BookStoreManagement.API.DTOs.Categories;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDBContext _context;

        public CategoryService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAll()
        {
            return await _context.Categories
                .Include(c => c.BookCategories)
                .Select(c => new CategoryResponseDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    HasBooks = c.BookCategories.Any()
                }).ToListAsync();
        }

        public async Task<Category?> GetById(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> Create(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> Update(int id, Category category)
        {
            if (id != category.CategoryId) return false;
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            // Chặn xóa nếu danh mục đang gắn với sách
            bool hasBooksLinked = await _context.BookCategories.AnyAsync(bc => bc.CategoryId == id);

            if (hasBooksLinked)
            {
                throw new InvalidOperationException("Cannot delete this category because it contains books.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}