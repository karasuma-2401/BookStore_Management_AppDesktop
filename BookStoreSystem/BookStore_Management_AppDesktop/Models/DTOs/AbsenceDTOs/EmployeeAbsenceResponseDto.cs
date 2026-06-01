namespace BookStore_Management_AppDesktop.Models.DTOs.AbsenceDTOs
{
    /// <summary>
    /// Response DTO for displaying absence records
    /// </summary>
    public class EmployeeAbsenceResponseDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string ShiftName { get; set; } = string.Empty;
        public DateTime WorkDate { get; set; }
        public string Status { get; set; } = string.Empty; // Scheduled, Present, Absent, Compensated
        public string? AbsenceReason { get; set; }
        public DateTime? CheckInTime { get; set; }
        public bool IsPaid { get; set; }
        public string ApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Denied
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}
