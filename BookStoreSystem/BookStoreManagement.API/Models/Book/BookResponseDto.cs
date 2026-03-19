namespace BookStoreManagement.API.Models.Book
{
    public class BookResponseDto
    {
        public int BookId { get; set; }

        public string Title { get; set; } = string.Empty;

        public int? AuthorId { get; set; }

        public string? AuthorName { get; set; }

        public int Quantity { get; set; }

        public string ImagePath { get; set; } = string.Empty;
    }
}