using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("vouchers")]
    public class Voucher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("voucher_id")]
        public int VoucherId { get; set; }

        [Required]
        [Column("code")]
        [MaxLength(50)]
        public string Code { get; set; } = null!;

        [Column("discount_percent")]
        public int? DiscountPercent { get; set; }

        [Column("discount_amount", TypeName = "decimal(10,2)")]
        public decimal? DiscountAmount { get; set; }

        [Column("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        [Column("usage_limit")]
        public int? UsageLimit { get; set; }

        [Column("used_count")]
        public int UsedCount { get; set; } = 0;
    }
}