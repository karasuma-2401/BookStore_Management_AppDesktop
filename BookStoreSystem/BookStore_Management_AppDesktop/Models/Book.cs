using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace BookStore_Management_AppDesktop.Models
{
    public class Book : ObservableObject
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; } 
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }

        public string CategoryNames { get; set; } = "Uncategorized";

        public List<int> CategoryIds { get; set; } = new();
    }
}