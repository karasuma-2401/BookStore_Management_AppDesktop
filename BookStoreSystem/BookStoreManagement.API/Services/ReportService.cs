using BookStoreManagement.API.Data;
using BookStoreManagement.API.Handlers;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Report;
using Microsoft.EntityFrameworkCore;
public class ReportService : IReportService
{
    private readonly ApplicationDBContext _context;

    public ReportService(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<ReportSummaryDto> GetMonthlyReport(int month, int year)
    {
        var fromDate = DateTime.SpecifyKind(new DateTime(year, month, 1), DateTimeKind.Utc);
        var toDate = fromDate.AddMonths(1); 
        var invoices = await _context.Invoices
            .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Book)
            .Where(i => i.InvoiceDate >= fromDate && i.InvoiceDate < toDate)
            .ToListAsync();

        var imports = await _context.Imports
            .Include(i => i.ImportDetails)
            .Where(i => i.ImportDate >= fromDate && i.ImportDate < toDate)
            .ToListAsync();

        var totalRevenue = invoices
            .SelectMany(i => i.InvoiceDetails)
            .Sum(d => d.Quantity * d.SalePrice);

        var totalImportCost = imports
            .SelectMany(i => i.ImportDetails)
            .Sum(d => d.Quantity * d.ImportPrice);

        var totalBooksSold = invoices
            .SelectMany(i => i.InvoiceDetails)
            .Sum(d => d.Quantity);
        var profit = totalRevenue - totalImportCost;
        var dailyData = invoices
            .GroupBy(i => i.InvoiceDate.Day)
            .Select(g => new DailyRevenueDto
            {
                Day = g.Key,
                Revenue = g.SelectMany(i => i.InvoiceDetails)
                           .Sum(d => d.Quantity * d.SalePrice)
            })
            .OrderBy(x => x.Day)
            .ToList();

        var topBooks = invoices
            .SelectMany(i => i.InvoiceDetails)
            .GroupBy(d => d.Book.Title)
            .Select(g => new TopBookDto
            {
                Title = g.Key,
                TotalSold = g.Sum(x => x.Quantity),
                RevenueGenerated = g.Sum(x => x.Quantity * x.SalePrice)
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(5)
            .ToList();

        return new ReportSummaryDto
        {
            TotalRevenue = totalRevenue,
            TotalImportCost = totalImportCost,
            Profit = profit,
            TotalBooksSold = totalBooksSold,
            DailyData = dailyData,
            TopSellingBooks = topBooks
        };
    }
}