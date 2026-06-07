using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models.DTOs.ImportDTOs
{
    public class ImportDetailResponseDto
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
        public int? PublishYear { get; set; }
        public decimal TotalPrice => Quantity * ImportPrice;
    }

    public class ImportResponseDto
    {
        public int ImportId { get; set; }
        public DateTime ImportDate { get; set; }
        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;
        public List<ImportDetailResponseDto> Details { get; set; } = new List<ImportDetailResponseDto>();
    }
}
