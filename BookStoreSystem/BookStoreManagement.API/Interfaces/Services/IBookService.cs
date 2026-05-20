using BookStoreManagement.API.Models.Book;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IBookService
    {
        Task<object> GetBooks(
                int? categoryId,
                int? authorId,
                string? keyword,
                string? sortBy,
                string? sortOrder,
                int page,
                int pageSize);
        Task<BookResponseDto?> GetBookById(int id);
        Task<Book> CreateBook(Book book, List<int> categoryIds);
        Task<bool> UpdateBook(int id, BookUpdateDto dto);
        Task<bool> DeleteBook(int id);
    }
}