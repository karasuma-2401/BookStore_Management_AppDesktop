using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models.DTOs
{
    public class EmployeeUpdateDto
    {
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; } 
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Salary { get; set; }
    }
}