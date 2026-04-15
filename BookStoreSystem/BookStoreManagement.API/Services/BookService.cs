using BookStoreManagement.API.Data;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Book;
using BookStoreManagement.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDBContext _context;

        public BookService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookResponseDto>> GetBooks(int? categoryId, int? authorId, string? keyword)
        {
            var query = _context.Books
                .Include(b => b.Author)
                    .Include(b => b.BookCategories)
                        .ThenInclude(bc => bc.Category)
                .AsQueryable();
            if (authorId.HasValue)
            {
                query = query.Where(b => b.AuthorId == authorId.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(b =>
                    b.BookCategories.Any(bc => bc.CategoryId == categoryId.Value));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(b => b.Title.Contains(keyword));
            }

            var result = await query
                .Select(b => new BookResponseDto
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author != null ? b.Author.Name : null,
                    Quantity = b.Quantity,    
                    ImagePath = b.ImagePath,
                    BookCategories = string.Join(", ",
                        b.BookCategories.Select(bc => bc.Category.Name))
                })
                .ToListAsync();

            return result;
        }

        public async Task<BookResponseDto?> GetBookById(int id)
        {
            return await _context.Books
                .Include(b => b.Author) 
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category) 
                .Where(b => b.BookId == id)
                .Select(b => new BookResponseDto
                {
                    BookId = b.BookId,
                    Title = b.Title,

                    AuthorId = b.AuthorId,
                    AuthorName = b.Author != null ? b.Author.Name : null,

                    Quantity = b.Quantity,
                    ImagePath = b.ImagePath,

                    BookCategories = string.Join(", ",
                        b.BookCategories.Select(bc => bc.Category.Name))
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Book> CreateBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> UpdateBook(int id, Book book)
        {
            if (id != book.BookId)
                return false;

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                if (!_context.Books.Any(e => e.BookId == id))
                    return false;
                else
                    throw;
            }

            return true;
        }

        public async Task<bool> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}