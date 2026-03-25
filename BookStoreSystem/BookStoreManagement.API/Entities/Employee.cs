using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("employees")]
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("employee_id")]
        public int EmployeeId {get; set;}

        [Column("user_id")]
        public int UserId {get; set;}

        [Required]
        [Column("full_name")]
        [MaxLength(200)]
        public string FullName {get; set;} = string.Empty;

        [Column("age")]
        public int Age {get; set;}

        [Column("phone")]
        [MaxLength(20)]
        public string Phone {get; set;} = string.Empty;

        [Column("address")]
        public string Address {get; set;} = string.Empty;

        [Column("salary", TypeName ="decimal(12,2)")]
        public decimal Salary {get; set;}

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public ICollection<EmployeeShift> EmployeeShifts = new List<EmployeeShift>();
    }
}