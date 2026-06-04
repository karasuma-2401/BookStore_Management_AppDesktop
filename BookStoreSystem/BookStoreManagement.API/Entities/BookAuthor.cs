using System.ComponentModel.DataAnnotations.Schema;
namespace BookStoreManagement.API.Models.Entities
{
    [Table("book_authors")]
    public class BookAuthor
    {
        [Column("book_id")]
        public int BookId { get; set; }
        public Book Book { get; set; }
        
        [Column("author_id")]
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}