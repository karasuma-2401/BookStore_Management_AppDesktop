using BookStoreManagement.API.Data;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Services.Interfaces;

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

            var change = 0;

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
    }
}