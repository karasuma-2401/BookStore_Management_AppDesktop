namespace BookStoreManagement.API.Models.Shift
{
    public class PayslipDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }

        public decimal Salary { get; set; } // Lương cứng
        public int TotalAssignedShifts { get; set; } // Tổng số ca được giao
        public int WorkedShifts { get; set; } // Số ca làm hợp lệ
        public int AbsentShifts { get; set; } // Số ca bị vắng/phạt
        public decimal ActualSalary { get; set; } // Lương thực nhận
    }
}