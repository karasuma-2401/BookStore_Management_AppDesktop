namespace BookStore_Management_AppDesktop.Models.DTOs.ShiftDTOs
{
    /// <summary>
    /// Response DTO for shift information
    /// </summary>
    public class ShiftResponseDto
    {
        public int ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
