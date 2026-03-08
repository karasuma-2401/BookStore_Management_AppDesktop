using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models
{
    [Table("invoice_details")]
    public class InvoiceDetail
    {
        [Key]
        [Column("id")]
        public int Id {get; set;}

        [Column("invoice_id")]
        public int InvoiceId {get; set;}

        [Column("book_id")]
        public int BookId {get; set;}

        [Column("quantity")]
        public int Quantity {get; set;}

        [Column("sale_price", TypeName = "decimal(12,2)")]
        public decimal SalePrice {get; set;}

        [ForeignKey("InvoiceId")]
        public Invoice Invoice {get; set;} = null!;

        [ForeignKey("BookId")]
        public Book Book {get; set; } = null!;
    }
}