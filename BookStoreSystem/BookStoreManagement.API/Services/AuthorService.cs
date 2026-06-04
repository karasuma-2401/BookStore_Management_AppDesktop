using BookStoreManagement.API.Data;
using BookStoreManagement.API.DTOs.Authors;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDBContext _context;

        public AuthorService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuthorResponseDto>> GetAll()
        {
            var authors = await _context.Authors
                .Include(a => a.BookAuthors)
                .ToListAsync();

            return authors.Select(a => new AuthorResponseDto
            {
                AuthorId = a.AuthorId,
                Name = a.Name,
                HasBooks = a.BookAuthors.Any()
            }).ToList();
        }

        public async Task<Author?> GetById(int id)
        {
            return await _context.Authors.FindAsync(id);
        }

        public async Task<Author> Create(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<bool> Update(int id, Author author)
        {
            if (id != author.AuthorId) return false;

            _context.Entry(author).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return false;

            bool hasBooksLinked = await _context.BookAuthors.AnyAsync(ba => ba.AuthorId == id);

            if (hasBooksLinked)
            {
                throw new InvalidOperationException("Cannot delete this author because they are linked to existing books.");
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}