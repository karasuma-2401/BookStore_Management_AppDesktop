using BookStoreManagement.API.Data;
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

            // giả sử chưa có log → tạm set = 0
            var change = 0;

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
    }
}