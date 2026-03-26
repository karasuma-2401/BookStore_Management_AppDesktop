using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Services.Interfaces
{
    public interface IBookCategoryService
    {
        Task<BookCategory> Create(BookCategory bc);
        Task<bool> Delete(int bookId, int categoryId);
    }
}