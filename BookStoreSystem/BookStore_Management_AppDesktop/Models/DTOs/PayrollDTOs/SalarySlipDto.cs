namespace BookStore_Management_AppDesktop.Models.DTOs.PayrollDTOs
{
    /// <summary>
    /// Response DTO for individual payroll/salary slip
    /// </summary>
    public class SalarySlipDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BaseSalary { get; set; }
        public int TotalShiftsAssigned { get; set; }
        public int TotalShiftsWorked { get; set; }
        public int TotalShiftsAbsent { get; set; }
        public decimal AbsentDeduction { get; set; }
        public decimal CompensationBonus { get; set; }
        public decimal NetSalary { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
    }
}
