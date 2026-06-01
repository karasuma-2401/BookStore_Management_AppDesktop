namespace BookStore_Management_AppDesktop.Models.DTOs.AbsenceDTOs
{
    /// <summary>
    /// Request DTO for updating absence/compensation status
    /// </summary>
    public class AbsenceApprovalDto
    {
        public int EmployeeShiftId { get; set; }
        public string Status { get; set; } = string.Empty; // "Approved" or "Denied"
        public string? Notes { get; set; }
    }
}
