using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("inventory_reports")]
    public class InventoryReport
    {
        [Key]
        [Column("report_id")]
        public int ReportId { get; set; }

        [Column("month")]
        public int Month { get; set; }

        [Column("year")]
        public int Year { get; set; }

        [Column("book_id")]
        public int BookId { get; set; }

        [Column("opening_stock")]
        public int OpeningStock { get; set; }

        [Column("change_amount")]
        public int ChangeAmount { get; set; }

        [Column("closing_stock")]
        public int ClosingStock { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Book Book { get; set; }
    }
}