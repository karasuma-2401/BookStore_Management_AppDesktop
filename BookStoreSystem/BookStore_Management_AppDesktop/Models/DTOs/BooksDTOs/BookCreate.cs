using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models.DTOs.BookDTOs
{
    public class BookCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public int? AuthorId { get; set; }
        public string? Description { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public List<int> CategoryIds { get; set; } = new List<int>();

    }
}
