namespace BookStore_Management_AppDesktop.Models.DTOs.PayrollDTOs
{
    /// <summary>
    /// Response DTO for payroll summary information
    /// </summary>
    public class PayrollSummaryDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalEmployeesProcessed { get; set; }
        public decimal TotalPayrollAmount { get; set; }
        public int PendingAbsenceCount { get; set; }
        public decimal Averagesalary { get; set; }
    }
}
