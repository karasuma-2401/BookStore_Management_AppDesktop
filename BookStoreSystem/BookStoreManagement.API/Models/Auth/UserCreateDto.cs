using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Auth
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
