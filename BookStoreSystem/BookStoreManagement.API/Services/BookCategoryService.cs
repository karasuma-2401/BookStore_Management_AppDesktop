using BookStoreManagement.API.Data;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace BookStoreManagement.API.Services
{
    public class BookCategoryService : IBookCategoryService
    {
        private readonly ApplicationDBContext _context;

        public BookCategoryService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<BookCategory> Create(BookCategory bc)
        {
            var bookExists = await _context.Books.AnyAsync(b => b.BookId == bc.BookId);
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == bc.CategoryId);

            if (!bookExists || !categoryExists)
                throw new Exception("BookId or CategoryId doesn't exist");

            _context.BookCategories.Add(bc);
            await _context.SaveChangesAsync();
            return bc;
        }

        public async Task<bool> Delete(int bookId, int categoryId)
        {
            var bc = await _context.BookCategories.FindAsync(bookId, categoryId);
            if (bc == null) return false;

            _context.BookCategories.Remove(bc);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}