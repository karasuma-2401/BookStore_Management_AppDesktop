namespace BookStoreManagement.API.Models.Book
{
    public class BookCreateDto
    {
        public string Title { get; set; } = string.Empty;

        public int? AuthorId { get; set; }

        public int Quantity { get; set; }

        public string ImagePath { get; set; } = string.Empty;
    }
}