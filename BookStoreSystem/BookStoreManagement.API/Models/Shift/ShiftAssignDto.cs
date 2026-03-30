namespace BookStoreManagement.API.Models.Shift
{
    public class ShiftAssignDto
    {
        public int EmployeeId { get; set; }
        public int ShiftId { get; set; }
        public DateTime WorkDate { get; set; } = DateTime.Now;
    }
}
