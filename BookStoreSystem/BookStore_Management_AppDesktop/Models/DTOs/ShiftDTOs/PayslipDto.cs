namespace BookStore_Management_AppDesktop.Models.DTOs.ShiftDTOs
{
    public class PayslipDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }

        public decimal Salary { get; set; } // L??ng c?ng
        public int TotalAssignedShifts { get; set; } // T?ng s? ca ???c giao
        public int WorkedShifts { get; set; } // S? ca làm h?p l?
        public int AbsentShifts { get; set; } // S? ca b? v?ng/ph?t
        public decimal ActualSalary { get; set; } // L??ng th?c nh?n
    }
}
