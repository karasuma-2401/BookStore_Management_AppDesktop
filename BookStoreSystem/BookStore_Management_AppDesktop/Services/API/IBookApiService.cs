using BookStore_Management_AppDesktop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API
{
    public interface IBookApiService
    {
        Task<List<Book>> GetAllBooksAsync();

        Task<bool> CreateBookAsync(Book newBook);

       // Task<Book?> GetBookByIdAsync(int id);

        Task<bool> UpdateBookAsync(int id, Book updatedBook);

        Task<bool> DeleteBookAsync(int id);
    }
}