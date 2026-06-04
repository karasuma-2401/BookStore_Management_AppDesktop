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

        public async Task<object> GetBooks(
            int? categoryId,
            int? authorId,
            string? keyword,
            string? sortBy,
            string? sortOrder,
            int page,
            int pageSize)
        {
            var query = _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .AsQueryable();

            if (authorId.HasValue)
            {
                query = query.Where(b =>
                    b.BookAuthors.Any(ba => ba.AuthorId == authorId.Value));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(b =>
                    b.BookCategories.Any(bc => bc.CategoryId == categoryId.Value));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(b =>
                    EF.Functions.ILike(b.Title, $"%{keyword}%"));
            }

            query = sortBy?.ToLower() switch
            {
                "price" => sortOrder == "desc"
                    ? query.OrderByDescending(b => b.Price)
                    : query.OrderBy(b => b.Price),

                "title" => sortOrder == "desc"
                    ? query.OrderByDescending(b => b.Title)
                    : query.OrderBy(b => b.Title),

                _ => query.OrderBy(b => b.BookId)
            };

            var totalItems = await query.CountAsync();

            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookResponseDto
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    PublishYear = b.PublishYear,
                    AuthorNames = b.BookAuthors
                        .Select(ba => ba.Author.Name)
                        .ToList(),

                    AuthorIds = b.BookAuthors
                        .Select(ba => ba.AuthorId)
                        .ToList(),

                    Quantity = b.Quantity,
                    Price = b.Price,
                    Description = b.Description,
                    ImagePath = b.ImagePath,

                    CategoryNames = b.BookCategories
                        .Select(bc => bc.Category.Name)
                        .ToList(),

                    CategoryIds = b.BookCategories
                        .Select(bc => bc.CategoryId)
                        .ToList()
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
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .Where(b => b.BookId == id)
                .Select(b => new BookResponseDto
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    PublishYear = b.PublishYear,

                    AuthorNames = b.BookAuthors
                        .Select(ba => ba.Author.Name)
                        .ToList(),

                    AuthorIds = b.BookAuthors
                        .Select(ba => ba.AuthorId)
                        .ToList(),

                    Quantity = b.Quantity,
                    Price = b.Price,
                    Description = b.Description,
                    ImagePath = b.ImagePath,

                    CategoryNames = b.BookCategories
                        .Select(bc => bc.Category.Name)
                        .ToList(),

                    CategoryIds = b.BookCategories
                        .Select(bc => bc.CategoryId)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BookResponseDto?> CreateBook(Book book, List<int> authorIds, List<int> categoryIds)
        {
            var validAuthors = await _context.Authors
                .Where(a => authorIds.Contains(a.AuthorId))
                .Select(a => a.AuthorId)
                .ToListAsync();

            if (validAuthors.Count != authorIds.Distinct().Count())
                throw new KeyNotFoundException("Invalid AuthorIds");

            var validCategories = await _context.Categories
                .Where(c => categoryIds.Contains(c.CategoryId))
                .Select(c => c.CategoryId)
                .ToListAsync();

            if (validCategories.Count != categoryIds.Distinct().Count())
                throw new KeyNotFoundException("Invalid CategoryIds");

            book.Quantity = 0;

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var bookAuthors = authorIds.Distinct()
                .Select(aid => new BookAuthor
                {
                    BookId = book.BookId,
                    AuthorId = aid
                });

            await _context.BookAuthors.AddRangeAsync(bookAuthors);

            var bookCategories = categoryIds.Distinct()
                .Select(cid => new BookCategory
                {
                    BookId = book.BookId,
                    CategoryId = cid
                });

            await _context.BookCategories.AddRangeAsync(bookCategories);

            await _context.SaveChangesAsync();

            return await GetBookById(book.BookId);
        }

        public async Task<bool> UpdateBook(int id, BookUpdateDto dto)
        {
            var book = await _context.Books
                .Include(b => b.BookAuthors)
                .Include(b => b.BookCategories)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
                return false;

            book.Title = dto.Title;
            book.Description = dto.Description;
            book.ImagePath = dto.ImagePath;
            book.Quantity = dto.Quantity;
            book.PublishYear = dto.PublishYear;

            // ?? UPDATE AUTHORS
            var newAuthorIds = dto.AuthorIds.Distinct().ToList();
            var oldAuthorIds = book.BookAuthors.Select(ba => ba.AuthorId).ToList();

            var removeAuthors = book.BookAuthors
                .Where(ba => !newAuthorIds.Contains(ba.AuthorId));

            _context.BookAuthors.RemoveRange(removeAuthors);

            var addAuthors = newAuthorIds
                .Where(id => !oldAuthorIds.Contains(id))
                .Select(id => new BookAuthor
                {
                    BookId = book.BookId,
                    AuthorId = id
                });

            await _context.BookAuthors.AddRangeAsync(addAuthors);

            var newCategoryIds = dto.CategoryIds.Distinct().ToList();
            var oldCategoryIds = book.BookCategories.Select(bc => bc.CategoryId).ToList();

            var removeCategories = book.BookCategories
                .Where(bc => !newCategoryIds.Contains(bc.CategoryId));

            _context.BookCategories.RemoveRange(removeCategories);

            var addCategories = newCategoryIds
                .Where(cid => !oldCategoryIds.Contains(cid))
                .Select(cid => new BookCategory
                {
                    BookId = id,
                    CategoryId = cid
                });

            await _context.BookCategories.AddRangeAsync(addCategories);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return false;

            book.IsDeleted = true;
            book.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}