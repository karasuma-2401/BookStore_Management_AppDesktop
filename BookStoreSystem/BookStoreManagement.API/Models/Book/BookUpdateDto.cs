namespace BookStoreManagement.API.Models.Book
{
    public class BookUpdateDto
    {
        public string Title { get; set; } = string.Empty;

        public int? AuthorId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string? Description { get; set; }
        public string ImagePath { get; set; } = string.Empty;
    }
}