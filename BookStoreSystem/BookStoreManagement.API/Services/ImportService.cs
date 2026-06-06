using BookStoreManagement.API.Data;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace BookStoreManagement.API.Services
{
    public class ImportService : IImportService
    {
        private readonly ApplicationDBContext _context;
        private readonly ISettingService _settingService;

        public ImportService(ApplicationDBContext context, ISettingService settingService)
        {
            _context = context;
            _settingService = settingService;
        }
        

        public async Task<ImportResponseDto> CreateImport(ImportCreateDto dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var import = new Import
                {
                    UserId = userId,
                    ImportDate = DateTime.UtcNow
                };

                _context.Imports.Add(import);
                await _context.SaveChangesAsync();

                var details = new List<ImportDetail>();
                var minImport = await _settingService.GetInt("SLNHAPTT");
                var maxStockToImport = await _settingService.GetInt("SLTONTD");
                var priceRate = await _settingService.GetDecimal("GIABAN");
                var totalQuantity = dto.Details.Sum(x => x.Quantity);
                if (totalQuantity < minImport)
                    throw new Exception($"Total import quantity must be at least {minImport}");

                foreach (var item in dto.Details)
                {
                    var book = await _context.Books.FindAsync(item.BookId);

                    if (book == null)
                        throw new Exception($"BookId {item.BookId} isn't exist");
                    if (book.Quantity >= maxStockToImport)
                        throw new Exception($"Cannot import book '{book.Title}' because stock >= {maxStockToImport}");


                    book.Quantity += item.Quantity;
                    book.Price = item.ImportPrice * priceRate;

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
                        PublishYear = _context.Books
                            .Where(b => b.BookId == d.BookId)
                            .Select(b => b.PublishYear)
                            .FirstOrDefault() ?? 0,
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
                .Include(i => i.User)
                    .ThenInclude(u => u.Employee)
                .Include(i => i.ImportDetails)
                    .ThenInclude(d => d.Book)
                .Select(i => new ImportResponseDto
                {
                    ImportId = i.ImportId,
                    ImportDate = i.ImportDate,
                    UserId = i.UserId,
                    UserName = i.User.Employee.FullName,
                    Details = i.ImportDetails.Select(d => new ImportDetailResponseDto
                    {
                        BookId = d.BookId,
                        BookTitle = d.Book.Title,
                        PublishYear = d.Book.PublishYear ?? 0,
                        Quantity = d.Quantity,
                        ImportPrice = d.ImportPrice
                    }).ToList()
                })
                .ToListAsync();
        }
        public async Task<ImportResponseDto?> GetImportById(int id)
        {
            var import = await _context.Imports
                .Include(i => i.User)
                    .ThenInclude(u => u.Employee)
                .Include(i => i.ImportDetails)
                    .ThenInclude(d => d.Book)
                .FirstOrDefaultAsync(i => i.ImportId == id);

            if (import == null) return null;

            return new ImportResponseDto
            {
                ImportId = import.ImportId,
                ImportDate = import.ImportDate,
                UserId = import.UserId,
                UserName = import.User.Employee.FullName,
                Details = import.ImportDetails.Select(d => new ImportDetailResponseDto
                {
                    BookId = d.BookId,
                    BookTitle = d.Book.Title,
                    PublishYear = d.Book.PublishYear ?? 0,
                    Quantity = d.Quantity,
                    ImportPrice = d.ImportPrice
                }).ToList()
            };
        }
    }
}