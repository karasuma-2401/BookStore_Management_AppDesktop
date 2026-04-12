using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models.DTOs
{
    public class BookResponseDto
    {
        public int BookId { get; set; }
        public string? Title { get; set; }
        public int? AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImagePath { get; set; }

        public string? Description { get; set; }
    }
}
