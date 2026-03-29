namespace BookStoreManagement.API.Models.Shift
{
    public class ShiftTimeUpdateDto
    {
        public required TimeSpan StartTime { get; set; }
        public required TimeSpan EndTime { get; set; }
    }
}
