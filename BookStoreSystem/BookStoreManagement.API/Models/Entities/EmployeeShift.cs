using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("employee_shifts")]
    public class EmployeeShift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id {get; set;}

        [Column("employee_id")]
        public int EmployeeId {get; set;}

        [Column("shift_id")]
        public int ShiftId {get; set;}

        [Column("work_date")]
        public DateTime WorkDate {get; set;}

        [ForeignKey("EmployeeId")]
        public Employee Employee {get; set;} = null!;

        [ForeignKey("ShiftId")]
        public Shift Shift {get; set;} = null!;
    }
}