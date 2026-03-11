using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models
{
    public class Book
    {
       
        public string Title { get; set; }
        public string Author { get; set; }
        public string Price { get; set; }
        public string CoverImagePath { get; set; }
    }
}