namespace BookStore_Management_AppDesktop.Models.DTOs.ShiftDTOs
{
    public class ShiftAssignDto
    {
        public int EmployeeId { get; set; }
        public int ShiftId { get; set; }
        public DateTime WorkDate { get; set; } = DateTime.Now;
    }
}
