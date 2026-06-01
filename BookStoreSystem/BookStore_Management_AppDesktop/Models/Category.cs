using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;

        public override string ToString()
        {
            return Name;
        }
    }
}
