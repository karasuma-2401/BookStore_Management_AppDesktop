using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs;
using BookStore_Management_AppDesktop.Models.DTOs.BookDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.Book
{
    public interface IBookApiService
    {
        Task<PagedResponse<Book>> GetAllBooksAsync(BookQueryParameters queryParams, CancellationToken ct = default);
        Task<Book?> CreateBookAsync(Book newBook);

        Task<Book?> GetBookByIdAsync(int id);

        Task<bool> UpdateBookAsync(int id, Book updatedBook);

        Task<bool> DeleteBookAsync(int id);
    }
}