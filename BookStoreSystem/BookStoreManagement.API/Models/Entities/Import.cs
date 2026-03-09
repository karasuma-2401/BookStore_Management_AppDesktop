using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("imports")]
    public class Import
    {
        [Key]
        [Column("import_id")]
        public int ImportId { get; set;}

        [Required]
        [Column("import_date")]
        public DateTime ImportDate = DateTime.UtcNow;

        [Column("user_id")]
        public int UserId {get; set;}

        [ForeignKey("UserId")]
        public User User {get; set;} = null!;

        public ICollection<ImportDetail> ImportDetails {get; set;} = new List<ImportDetail>();

    }
}