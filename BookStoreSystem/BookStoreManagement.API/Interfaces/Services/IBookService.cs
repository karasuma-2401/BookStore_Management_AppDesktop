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
            int pageSize,
            bool includeOutOfStock = false);

        Task<BookResponseDto?> GetBookById(int id);
        Task<BookResponseDto?> CreateBook(
            Book book,
            List<int> authorIds,
            List<int> categoryIds
        );

        Task<bool> UpdateBook(int id, BookUpdateDto dto);

        Task<bool> DeleteBook(int id);
    }
}