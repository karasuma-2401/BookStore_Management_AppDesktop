using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("books")]
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("book_id")]
        public int BookId {get; set;}

        [Required]
        [MaxLength(255)]
        [Column("title")]
        public string Title {get; set;} = string.Empty;

        [Column("author_id")]
        public int? AuthorId {get; set;}

        [Column("quantity")]
        public int Quantity {get; set;} = 0;

        [Column("image_path")]
        public string ImagePath { get; set; } = string.Empty;

        [ForeignKey("AuthorId")]
        public Author? Author {get; set;}
        
        public ICollection<BookCategory> BookCategories {get; set;} = new List <BookCategory>();
    }
}