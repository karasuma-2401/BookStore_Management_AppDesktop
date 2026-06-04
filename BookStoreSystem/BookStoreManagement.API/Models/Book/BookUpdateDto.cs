namespace BookStoreManagement.API.Models.Book
{
    public class BookUpdateDto
    {
        public string Title { get; set; } = string.Empty;

        public List<int> AuthorIds { get; set; } = new();
        public int PublishYear { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public List<int> CategoryIds { get; set; } = new();
    }
}