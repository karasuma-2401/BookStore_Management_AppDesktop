using BookStoreManagement.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("invoices")]
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("invoice_id")]
        public int InvoiceId {get; set;}

        [Column("customer_id")]
        public int? CustomerId {get; set;}

        [Column("user_id")]
        public int UserId {get; set;}

        [Column("invoice_date")]
        public DateTime InvoiceDate {get; set;} = DateTime.UtcNow;

        [Column("total", TypeName = "decimal(12,2)")]
        public decimal Total {get; set;}

        [ForeignKey("CustomerId")]
        public Customer? Customer {get; set;}
        
        [Column("voucher_id")]
        public int? VoucherId { get; set; }

        [Column("amount_paid", TypeName = "decimal(12,2)")]
        public decimal AmountPaid { get; set; } = 0;

        [Column("status")]
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Completed;

        [ForeignKey("UserId")]
        public User User {get; set;}
        
        [ForeignKey("VoucherId")]
        public Voucher? Voucher { get; set; }

        public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    }
}