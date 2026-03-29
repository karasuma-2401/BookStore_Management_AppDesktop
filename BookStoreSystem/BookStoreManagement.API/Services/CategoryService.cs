using Microsoft.EntityFrameworkCore;
using BookStoreManagement.API.Data;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Services.Interfaces;

namespace BookStoreManagement.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDBContext _context;

        public CategoryService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await _context.Categories.ToListAsync();
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

        public async Task<bool> Delete(int id)
        {
            var c = await _context.Categories.FindAsync(id);
            if (c == null) return false;

            _context.Categories.Remove(c);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}