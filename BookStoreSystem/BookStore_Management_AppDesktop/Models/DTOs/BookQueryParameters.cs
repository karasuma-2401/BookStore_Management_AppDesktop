
namespace BookStore_Management_AppDesktop.Models.DTOs
{
    public class BookQueryParameters
    {
        public string? Keyword { get; set; }

        public string? SortBy { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
