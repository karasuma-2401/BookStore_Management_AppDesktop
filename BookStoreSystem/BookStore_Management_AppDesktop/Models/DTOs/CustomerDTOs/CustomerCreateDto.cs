namespace BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs
{
    public class CustomerCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}
