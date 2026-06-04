namespace BookStoreManagement.API.DTOs.Authors
{
    public class AuthorResponseDto
    {
        public int AuthorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool HasBooks { get; set; } 
    }
}