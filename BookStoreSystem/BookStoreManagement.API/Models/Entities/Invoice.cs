using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("invoices")]
    public class Invoice
    {
        [Key]
        [Column("invoice_id")]
        public int InvoiceId {get; set;}

        [Column("customer_id")]
        public int? CustomerId {get; set;}

        [Column("user_id")]
        public int? UserId {get; set;}

        [Column("invoice_date")]
        public DateTime InvoiceDate {get; set;} = DateTime.UtcNow;

        [Column("total", TypeName = "decimal(12,2)")]
        public decimal Total {get; set;}

        [ForeignKey("CustomerId")]
        public Customer Customer {get; set;} = null!;
        
        [ForeignKey("UserId")]
        public User User {get; set;} = null!;
    }
}