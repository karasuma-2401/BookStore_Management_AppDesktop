namespace BookStoreManagement.API.Models.Employee
{
    public class EmployeeResponseDto
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public int Age { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public decimal Salary { get; set; }
    }
}
