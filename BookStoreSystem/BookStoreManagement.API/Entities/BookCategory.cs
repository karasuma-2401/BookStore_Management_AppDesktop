using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("book_categories")]
    public class BookCategory
    {
        [Column("book_id")]
        public int BookId { get; set; }

        [Column("category_id")]
        public int CategoryId {get; set;}

        [ForeignKey("BookId")]
        public Book Book {get; set;} = null!;

        [ForeignKey("CategoryId")]
        public Category Category {get; set; } = null!;
    }
}