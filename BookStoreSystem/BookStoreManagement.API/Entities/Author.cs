using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("authors")]
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("author_id")]
        public int AuthorId {get; set;}

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name {get; set; } = string.Empty;

        public ICollection<BookAuthor> BookAuthors { get; set; }
    }
}