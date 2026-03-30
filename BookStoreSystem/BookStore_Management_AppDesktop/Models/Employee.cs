using System.Text.Json.Serialization;

namespace BookStore_Management_AppDesktop.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; } // Khớp [Column("employee_id")]
        public int UserId { get; set; }     // Khớp [Column("user_id")]
        public string FullName { get; set; } = string.Empty; // Khớp [Column("full_name")]
        public int Age { get; set; }        // Khớp [Column("age")]
        public string Phone { get; set; } = string.Empty;    // Khớp [Column("phone")]
        public string Address { get; set; } = string.Empty;  // Khớp [Column("address")]
        public decimal Salary { get; set; } // Khớp [Column("salary")]
    }
}