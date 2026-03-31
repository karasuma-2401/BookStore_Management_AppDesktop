using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models.DTOs
{
    public class EmployeeUpdateDto : EmployeeCreateDto
    {
        public int EmployeeId { get; set; }
    }
}