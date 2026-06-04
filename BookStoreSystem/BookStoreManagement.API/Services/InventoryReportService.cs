using BookStoreManagement.API.Data;
using BookStoreManagement.API.Models.DTOs;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Services
{
    public class InventoryReportService : IInventoryReportService
    {
        private readonly ApplicationDBContext _context;

        public InventoryReportService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<InventoryReport> CreateReport(int month, int year, int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                throw new Exception("Book not found");

            var openingStock = book.Quantity;
            var totalImport = await _context.ImportDetails
            .Where(x => x.BookId == bookId
                && x.Import.ImportDate.Month == month
                && x.Import.ImportDate.Year == year)
            .SumAsync(x => (int?)x.Quantity) ?? 0;
                    var totalSold = await _context.InvoiceDetails
            .Where(x => x.BookId == bookId
                && x.Invoice.InvoiceDate.Month == month
                && x.Invoice.InvoiceDate.Year == year)
            .SumAsync(x => (int?)x.Quantity) ?? 0;
            var change = totalImport - totalSold;

            var report = new InventoryReport
            {
                Month = month,
                Year = year,
                BookId = bookId,
                OpeningStock = openingStock,
                ChangeAmount = change,
                ClosingStock = openingStock + change
            };

            _context.InventoryReports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }
        public async Task<IEnumerable<InventoryReportResponseDTO>> GetReports(int month, int year)
        {
            return await _context.InventoryReports
                .Where(x => x.Month == month && x.Year == year)
                .Select(x => new InventoryReportResponseDTO
                {
                    ReportId = x.ReportId,
                    Month = x.Month,
                    Year = x.Year,
                    BookId = x.BookId,
                    OpeningStock = x.OpeningStock,
                    ChangeAmount = x.ChangeAmount,
                    ClosingStock = x.ClosingStock
                })
                .ToListAsync();
        }

        public async Task<InventoryReportResponseDTO?> GetById(int id)
        {
            return await _context.InventoryReports
                .Where(x => x.ReportId == id)
                .Select(x => new InventoryReportResponseDTO
                {
                    ReportId = x.ReportId,
                    Month = x.Month,
                    Year = x.Year,
                    BookId = x.BookId,
                    OpeningStock = x.OpeningStock,
                    ChangeAmount = x.ChangeAmount,
                    ClosingStock = x.ClosingStock
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<InventoryReportResponseDTO>> GetByBook(int bookId)
        {
            return await _context.InventoryReports
                .Where(x => x.BookId == bookId)
                .Select(x => new InventoryReportResponseDTO
                {
                    ReportId = x.ReportId,
                    Month = x.Month,
                    Year = x.Year,
                    BookId = x.BookId,
                    OpeningStock = x.OpeningStock,
                    ChangeAmount = x.ChangeAmount,
                    ClosingStock = x.ClosingStock
                })
                .ToListAsync();
        }
    }
}