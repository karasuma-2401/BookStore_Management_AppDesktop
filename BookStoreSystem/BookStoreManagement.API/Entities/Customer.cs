using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("customers")]
    public class Customer {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("customer_id")]
        public int CustomerId {get; set;}

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name {get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("phone")]
        public  string? Phone {get; set; }

        [MaxLength(150)]
        [Column("email")]
        public string? Email {get; set;}

        [Column("address")]
        public string? Address {get; set;}

        [Column("debt", TypeName = "decimal(12,2)")]
        public decimal Debt {get; set;} = 0;

        public ICollection<Invoice> Invoices {get; set; } = new List<Invoice>();
        public ICollection<Payment> Payments {get; set; } = new List<Payment>();

    }
}