namespace BookStore_Management_AppDesktop.Models.DTOs.AbsenceDTOs
{
    /// <summary>
    /// Request DTO for bulk approval of absences
    /// </summary>
    public class BulkAbsenceApprovalDto
    {
        public List<int> EmployeeShiftIds { get; set; } = new();
        public string Status { get; set; } = string.Empty; // "Approved" or "Denied"
        public string? Notes { get; set; }
    }
}
