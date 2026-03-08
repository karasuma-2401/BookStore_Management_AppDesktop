using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models
{
    [Table("import_details")]
    public class ImportDetail
    {
        [Key]
        [Column("id")]
        public int Id {get; set;}

        [Column("import_id")]
        public int ImportId {get; set;}

        [Column("book_id")]
        public int BookId {get; set;}

        [Column("quantity")]
        public int Quantity {get; set;}

        [Column("import_price", TypeName = "decimal(12,2)")]
        public decimal ImportPrice {get; set;}

        [ForeignKey("ImportId")]
        public Import Import {get; set;} = null!;

        [ForeignKey("BookId")] 
        public Book Book {get; set;} = null!;
    }
}
