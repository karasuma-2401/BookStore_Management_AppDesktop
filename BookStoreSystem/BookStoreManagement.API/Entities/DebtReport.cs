using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("debt_reports")]
    public class DebtReport
    {
        [Key]
        [Column("report_id")]
        public int ReportId { get; set; }

        [Column("month")]
        public int Month { get; set; }

        [Column("year")]
        public int Year { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("opening_debt", TypeName = "decimal(12,2)")]
        public decimal OpeningDebt { get; set; }

        [Column("change_amount")]
        public decimal ChangeAmount { get; set; }

        [Column("closing_debt", TypeName = "decimal(12,2)")]
        public decimal ClosingDebt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Customer Customer { get; set; }
    }
}