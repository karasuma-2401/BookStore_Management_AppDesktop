namespace BookStoreManagement.API.Models.Auth
{
    public class UserResponseModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
