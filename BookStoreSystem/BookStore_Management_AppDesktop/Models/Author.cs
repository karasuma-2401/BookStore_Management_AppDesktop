using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool HasBooks { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
