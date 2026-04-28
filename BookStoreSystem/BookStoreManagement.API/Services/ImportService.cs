using BookStoreManagement.API.Data;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace BookStoreManagement.API.Services
{
    public class ImportService : IImportService
    {
        private readonly ApplicationDBContext _context;

        public ImportService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<ImportResponseDto> CreateImport(ImportCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var import = new Import
                {
                    UserId = dto.UserId,
                    ImportDate = DateTime.UtcNow
                };

                _context.Imports.Add(import);
                await _context.SaveChangesAsync();

                var details = new List<ImportDetail>();

                foreach (var item in dto.Details)
                {
                    var book = await _context.Books.FindAsync(item.BookId);

                    if (book == null)
                        throw new Exception($"BookId {item.BookId} không tồn tại");

                    book.Quantity += item.Quantity;
                    book.Price = item.ImportPrice;

                    details.Add(new ImportDetail
                    {
                        ImportId = import.ImportId,
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        ImportPrice = item.ImportPrice
                    });
                }

                _context.ImportDetails.AddRange(details);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ImportResponseDto
                {
                    ImportId = import.ImportId,
                    ImportDate = import.ImportDate,
                    UserId = import.UserId,
                    Details = details.Select(d => new ImportDetailResponseDto
                    {
                        BookId = d.BookId,
                        BookTitle = _context.Books
                            .Where(b => b.BookId == d.BookId)
                            .Select(b => b.Title)
                            .FirstOrDefault() ?? "",
                        Quantity = d.Quantity,
                        ImportPrice = d.ImportPrice
                    }).ToList()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<ImportResponseDto>> GetImports()
        {
            return await _context.Imports
                .Include(i => i.ImportDetails)
                    .ThenInclude(d => d.Book)
                .Select(i => new ImportResponseDto
                {
                    ImportId = i.ImportId,
                    ImportDate = i.ImportDate,
                    UserId = i.UserId,
                    Details = i.ImportDetails.Select(d => new ImportDetailResponseDto
                    {
                        BookId = d.BookId,
                        BookTitle = d.Book.Title,
                        Quantity = d.Quantity,
                        ImportPrice = d.ImportPrice
                    }).ToList()
                })
                .ToListAsync();
        }
        public async Task<ImportResponseDto?> GetImportById(int id)
        {
            var import = await _context.Imports
                .Include(i => i.ImportDetails)
                    .ThenInclude(d => d.Book)
                .FirstOrDefaultAsync(i => i.ImportId == id);

            if (import == null) return null;

            return new ImportResponseDto
            {
                ImportId = import.ImportId,
                ImportDate = import.ImportDate,
                UserId = import.UserId,
                Details = import.ImportDetails.Select(d => new ImportDetailResponseDto
                {
                    BookId = d.BookId,
                    BookTitle = d.Book.Title,
                    Quantity = d.Quantity,
                    ImportPrice = d.ImportPrice
                }).ToList()
            };
        }
    }
}