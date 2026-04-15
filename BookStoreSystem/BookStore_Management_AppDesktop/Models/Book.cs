using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string? Title { get; set; } = string.Empty;
        public int? AuthorId { get; set; }
        public int Quantity { get; set; }
        public string? AuthorName { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; } = null;

        public string? Description { get; set; }

    }
}