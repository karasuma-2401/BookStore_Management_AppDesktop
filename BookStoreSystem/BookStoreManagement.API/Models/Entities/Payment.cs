using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("payments")]
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId {get; set;}

        [Column("customer_id")]
        public int CustomerId {get; set;}

        [Column("user_id")]
        public int? UserId {get; set;}

        [Column("payment_date")]
        public DateTime PaymentDate {get; set;} = DateTime.UtcNow;

        [Column("amount", TypeName = "decimal(12,2)")]
        public decimal Amount {get; set;}

        [ForeignKey("CustomerId")]
        public Customer Customer {get; set;} = null!;

        [ForeignKey("UserId")]
        public User User {get; set;} = null!;
    }
}