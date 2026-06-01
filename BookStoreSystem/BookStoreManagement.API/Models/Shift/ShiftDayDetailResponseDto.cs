using System;
using System.Collections.Generic;

namespace BookStoreManagement.API.Models.Shift
{
    public class ShiftDayDetailResponseDto
    {
        public DateTime Date { get; set; }
        public List<ShiftDayItemDto> Shifts { get; set; } = new();
    }

    public class ShiftDayItemDto
    {
        public int? AssignmentId { get; set; } // Null if ca trống (no one assigned)
        public int ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string WorkTime { get; set; } = string.Empty; // e.g., "08:00 - 12:00"
        public int? EmployeeId { get; set; }
        public string? FullName { get; set; }
        public string Status { get; set; } = "Empty"; // Empty, Scheduled, Present, Late, Absent, Compensated
        public DateTime? CheckInTime { get; set; }
        public bool IsPaid { get; set; }
    }
}
