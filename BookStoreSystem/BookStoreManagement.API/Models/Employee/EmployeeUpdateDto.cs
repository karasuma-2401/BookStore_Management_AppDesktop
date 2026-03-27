namespace BookStoreManagement.API.Models.Employee
{
    public class EmployeeUpdateDto
    {
        public required string FullName { get; set; }
        public int Age { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public decimal Salary { get; set; }

    }
}
