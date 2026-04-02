using BookStoreManagement.API.Models.Book;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookResponseDto>> GetBooks(int? categoryId, int? authorId, string? keyword);
        Task<Book?> GetBookById(int id);
        Task<Book> CreateBook(Book book);
        Task<bool> UpdateBook(int id, Book book);
        Task<bool> DeleteBook(int id);
    }
}