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

        [Column("price", TypeName = "decimal(12,2)")]
        public decimal Price { get; set; } = 0;

        [Column("description", TypeName = "text")]
        public string? Description { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

        public ICollection<BookCategory> BookCategories {get; set;} = new List <BookCategory>();
    }
}