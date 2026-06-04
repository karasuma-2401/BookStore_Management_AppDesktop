using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace BookStore_Management_AppDesktop.Models
{
    public class Book : ObservableObject
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;

        // Author: 
        public List<int> AuthorIds { get; set; } = new();
        public List<string> AuthorNames { get; set; } = new();
        public string DisplayAuthorNames => AuthorNames != null && AuthorNames.Any()
            ? string.Join(", ", AuthorNames)
            : "Unknown Author";

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public int PublishYear { get; set; }

        // Category: 
        public List<string> CategoryNames { get; set; } = new();
        public List<int> CategoryIds { get; set; } = new();
        public string DisplayCategoryNames => CategoryNames != null && CategoryNames.Any()
            ? string.Join(", ", CategoryNames)
            : "Uncategorized";
    }
}