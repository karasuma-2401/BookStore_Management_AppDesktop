using BookStoreManagement.API.Data;
using BookStoreManagement.API.Models.DTOs;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Services
{
    public class DebtReportService : IDebtReportService
    {
        private readonly ApplicationDBContext _context;

        public DebtReportService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<DebtReport> CreateReport(int month, int year, int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                throw new Exception("Customer not found");

            var openingDebt = customer.Debt;
            var totalInvoice = await _context.Invoices
            .Where(x => x.CustomerId == customerId
                && x.InvoiceDate.Month == month
                && x.InvoiceDate.Year == year)
            .SumAsync(x => (decimal?)x.Total) ?? 0;
            var totalPayment = await _context.Payments
            .Where(x => x.CustomerId == customerId
                && x.PaymentDate.Month == month
                && x.PaymentDate.Year == year)
            .SumAsync(x => (decimal?)x.Amount) ?? 0;
            var change = totalInvoice - totalPayment;

            var report = new DebtReport
            {
                Month = month,
                Year = year,
                CustomerId = customerId,
                OpeningDebt = openingDebt,
                ChangeAmount = change,
                ClosingDebt = openingDebt + change
            };

            _context.DebtReports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }
        public async Task<IEnumerable<DebtReportResponseDTO>> GetReports(int month, int year)
        {
            return await _context.DebtReports
                .Where(x => x.Month == month && x.Year == year)
                .Select(x => new DebtReportResponseDTO
                {
                    ReportId = x.ReportId,
                    Month = x.Month,
                    Year = x.Year,
                    CustomerId = x.CustomerId,
                    OpeningDebt = x.OpeningDebt,
                    ChangeAmount = x.ChangeAmount,
                    ClosingDebt = x.ClosingDebt
                })
                .ToListAsync();
        }

        public async Task<DebtReportResponseDTO?> GetById(int id)
        {
            return await _context.DebtReports
                .Where(x => x.ReportId == id)
                .Select(x => new DebtReportResponseDTO
                {
                    ReportId = x.ReportId,
                    Month = x.Month,
                    Year = x.Year,
                    CustomerId = x.CustomerId,
                    OpeningDebt = x.OpeningDebt,
                    ChangeAmount = x.ChangeAmount,
                    ClosingDebt = x.ClosingDebt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DebtReportResponseDTO>> GetByCustomer(int customerId)
        {
            return await _context.DebtReports
                .Where(x => x.CustomerId == customerId)
                .Select(x => new DebtReportResponseDTO
                {
                    ReportId = x.ReportId,
                    Month = x.Month,
                    Year = x.Year,
                    CustomerId = x.CustomerId,
                    OpeningDebt = x.OpeningDebt,
                    ChangeAmount = x.ChangeAmount,
                    ClosingDebt = x.ClosingDebt
                })
                .ToListAsync();
        }
    }
}