using BookStoreManagement.API.DTOs.Authors;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorResponseDto>> GetAll(); 
        Task<Author?> GetById(int id);
        Task<Author> Create(Author author);
        Task<bool> Update(int id, Author author);
        Task<bool> Delete(int id);
    }
}