using BookStore_Management_AppDesktop.Models.DTOs.ShiftDTOs;
namespace BookStore_Management_AppDesktop.Services.API
{
    public interface IEmployeeShiftApiService
    {
        Task<List<EmployeeShiftResponseDto>> GetScheduleAsync(DateTime startDate, DateTime endDate, int? employeeId = null);
        Task<(bool Success, string Message)> AssignShiftAsync(ShiftAssignDto dto);
        Task<bool> DeleteAssignmentAsync(int id);
        Task<bool> ApproveCompensationAsync(int id);
        Task<bool> ApproveBulkCompensationAsync(List<int> assignmentIds);
        Task<PayslipDto?> GetPayrollAsync(int employeeId, int month, int year);
        Task<List<EmployeeShiftResponseDto>> GetAbsentShiftsAsync(DateTime startDate, DateTime endDate);
        Task<ShiftDayDetailResponseDto?> GetDayDetailAsync(DateTime date);
        Task<KioskCheckInResponseDto?> KioskCheckInAsync(int employeeId);
    }
}
