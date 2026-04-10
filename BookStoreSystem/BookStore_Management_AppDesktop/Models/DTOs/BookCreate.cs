using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models.DTOs
{
    public class BookCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public int? AuthorId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImagePath { get; set; }
    }
}
