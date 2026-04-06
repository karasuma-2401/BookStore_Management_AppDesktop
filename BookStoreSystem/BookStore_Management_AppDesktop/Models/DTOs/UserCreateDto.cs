using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models.DTOs
{
    public class UserCreateDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public string RoleId { get; set; }
        public required string Email { get; set; }

    }
}
