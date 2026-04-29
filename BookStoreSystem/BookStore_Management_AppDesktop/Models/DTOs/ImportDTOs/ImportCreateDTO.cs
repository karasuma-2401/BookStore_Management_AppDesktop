using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models.DTOs.ImportDTOs
{
    public class ImportCreateDTO
    {
        public int UserId { get; set; } 
        public List<ImportDetailCreateDTO> Details { get; set; } = new List<ImportDetailCreateDTO>();
    }

    public class ImportDetailCreateDTO
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
    }
}
