namespace BookStore_Management_AppDesktop.Models.Dtos
{
    public class LoginResponseModel
    {
        public string AccessToken { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }
}