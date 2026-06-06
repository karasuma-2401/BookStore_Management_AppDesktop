namespace BookStore_Management_AppDesktop.Models.DTOs.BookDTOs
{
    public class BookQueryParameters
    {
        public int? CategoryId { get; set; }
        public int? AuthorId { get; set; }
        public string? Keyword { get; set; }
        public string? SortBy { get; set; } = "price";
        public string? SortOrder { get; set; } = "asc"; 
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeOutOfStock { get; set; } = false;
    }
}
