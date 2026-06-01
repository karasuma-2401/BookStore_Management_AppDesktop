using System;

namespace BookStoreManagement.API.Models.Shift
{
    public class KioskCheckInResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string ShiftName { get; set; } = string.Empty;
        public string WorkTime { get; set; } = string.Empty;
        public DateTime? CheckInTime { get; set; }
        public string Status { get; set; } = string.Empty; // Present, Late, Scheduled, etc.
    }
}
