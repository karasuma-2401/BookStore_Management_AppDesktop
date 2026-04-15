using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; } = string.Empty;

        public override string ToString()
        {
            return Name;
        }
    }
}
