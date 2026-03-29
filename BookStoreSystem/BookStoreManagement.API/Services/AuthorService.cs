using Microsoft.EntityFrameworkCore;
using BookStoreManagement.API.Data;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Services.Interfaces;

namespace BookStoreManagement.API.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDBContext _context;

        public AuthorService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAll()
        {
            return await _context.Authors.ToListAsync();
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
            var a = await _context.Authors.FindAsync(id);
            if (a == null) return false;

            _context.Authors.Remove(a);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}