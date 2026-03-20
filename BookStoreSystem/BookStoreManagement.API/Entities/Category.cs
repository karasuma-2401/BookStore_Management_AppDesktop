using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("categories")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("category_id")]
        public int CategoryId {get; set;}

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name {get; set;} = string.Empty;

        public ICollection<BookCategory> BookCategories {get; set;} = new List<BookCategory>();
    }
}