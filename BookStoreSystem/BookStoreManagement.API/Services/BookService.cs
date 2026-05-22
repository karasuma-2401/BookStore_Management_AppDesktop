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

        public async Task<object> GetBooks(int? categoryId, int? authorId, string? keyword, string? sortBy, string? sortOrder, int page,  int pageSize)
        {
            var query = _context.Books.AsQueryable();

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

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "price":
                        query = sortOrder == "desc"
                            ? query.OrderByDescending(b => b.Price)
                            : query.OrderBy(b => b.Price);
                        break;

                    case "title":
                        query = sortOrder == "desc"
                            ? query.OrderByDescending(b => b.Title)
                            : query.OrderBy(b => b.Title);
                        break;

                    default:
                        query = query.OrderBy(b => b.BookId);
                        break;
                }
            }
            var totalItems = await query.CountAsync();

            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookResponseDto
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author.Name,
                    Quantity = b.Quantity,
                    Price = b.Price,
                    ImagePath = b.ImagePath,
                    CategoryNames = b.BookCategories
                    .Select(bc => bc.Category.Name)
                    .ToList(),
                    CategoryIds = b.BookCategories.Select(bc => bc.CategoryId).ToList()
                })
                .ToListAsync();

            return new
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                Data = books
            };
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
                    Price = b.Price,
                    Description = b.Description,
                    ImagePath = b.ImagePath,
                    CategoryNames = b.BookCategories
                    .Select(bc => bc.Category.Name)
                    .ToList(),
                    CategoryIds = b.BookCategories.Select(bc => bc.CategoryId).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Book> CreateBook(Book book, List<int> categoryIds)
        {
            book.Quantity = 0;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            var bookCategories = categoryIds.Select(cid => new BookCategory
            {
                BookId = book.BookId,
                CategoryId = cid
            }).ToList();

            _context.BookCategories.AddRange(bookCategories);
            await _context.SaveChangesAsync();

            return book;
        }

        public async Task<bool> UpdateBook(int id, BookUpdateDto dto)
        {
            var book = await _context.Books
                .Include(b => b.BookCategories)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
                return false;

            var categoryIds = dto.CategoryIds
                .Distinct()
                .ToList();

            var validCategoryIds = await _context.Categories
                .Where(c => categoryIds.Contains(c.CategoryId))
                .Select(c => c.CategoryId)
                .ToListAsync();

            if (validCategoryIds.Count != categoryIds.Count)
                throw new Exception("One or more CategoryId is invalid");

            book.Title = dto.Title;
            book.AuthorId = dto.AuthorId;
            book.Quantity = dto.Quantity;
            book.Description = dto.Description;
            book.ImagePath = dto.ImagePath;

            var existingCategoryIds = book.BookCategories
                .Select(bc => bc.CategoryId)
                .ToList();

            var toRemove = book.BookCategories
                .Where(bc => !categoryIds.Contains(bc.CategoryId))
                .ToList();

            _context.BookCategories.RemoveRange(toRemove);

            var toAdd = categoryIds
                .Where(cid => !existingCategoryIds.Contains(cid))
                .Select(cid => new BookCategory
                {
                    BookId = id,
                    CategoryId = cid
                });

            await _context.BookCategories.AddRangeAsync(toAdd);

            await _context.SaveChangesAsync();

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