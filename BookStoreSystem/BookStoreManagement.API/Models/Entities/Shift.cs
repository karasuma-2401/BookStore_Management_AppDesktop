using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("shifts")]
    public class Shift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("shifts")]
        private int ShiftId {get; set;}

        [Required]
        [MaxLength(50)]
        [Column("shift_name")]
        private string ShiftName {get; set;} = string.Empty;

        [Column("start_time")]
        private DateTime StartTime {get; set;}

        [Column("start_time")]
        private DateTime EndTime {get; set;}

        public ICollection<EmployeeShift> EmployeeShifts = new List<EmployeeShift>();

    }
}