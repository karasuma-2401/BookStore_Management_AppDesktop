namespace BookStoreManagement.API.Models.Shift
{
    public class EmployeeShiftResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ShiftName { get; set; } = string.Empty;
        public string WorkTime { get; set; } = string.Empty;
        public string WorkDate { get; set; }
    }
}
