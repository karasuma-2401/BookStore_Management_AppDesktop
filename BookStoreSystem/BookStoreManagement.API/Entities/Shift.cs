using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("shifts")]
    public class Shift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("shift_id")]
        public int ShiftId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("shift_name")]
        public string ShiftName { get; set; } = string.Empty;

        [Column("start_time")]
        public TimeSpan StartTime { get; set; }

        [Column("end_time")]
        public TimeSpan EndTime { get; set; }

        public ICollection<EmployeeShift> EmployeeShifts = new List<EmployeeShift>();

    }
}