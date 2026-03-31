using System.Text.Json.Serialization;

namespace BookStore_Management_AppDesktop.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }   
        public string FullName { get; set; } = string.Empty; 
        public int Age { get; set; }      
        public string Phone { get; set; } = string.Empty;   
        public string Address { get; set; } = string.Empty; 
        public decimal Salary { get; set; }
    }
}