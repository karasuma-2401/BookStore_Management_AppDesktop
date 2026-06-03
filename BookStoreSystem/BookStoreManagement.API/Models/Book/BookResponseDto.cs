namespace BookStoreManagement.API.Models.Book
{
    public class BookResponseDto
    {
        public int BookId { get; set; }

        public string Title { get; set; } = string.Empty;

        public List<int> AuthorIds { get; set; } = new List<int>();
        public List<string> AuthorNames { get; set; } = new List<string>();

        public int Quantity { get; set; }

        public decimal Price { get; set; }
        public string? Description { get; set; }

        public string ImagePath { get; set; } = string.Empty;
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<string> CategoryNames { get; set; } = new List<string>();
    }
}