namespace BookStoreManagement.API.DTOs.Books
{
    public class BookUpdateDto
    {
        public string Title { get; set; } = string.Empty;

        public int? AuthorId { get; set; }

        public int Quantity { get; set; }

        public string ImagePath { get; set; } = string.Empty;
    }
}