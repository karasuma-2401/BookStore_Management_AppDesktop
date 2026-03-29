using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Models.Shift;

namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IEmployeeShift
    {
        Task<string?> AssignShiftAsync(ShiftAssignDto dto);
        Task<IEnumerable<EmployeeShiftResponseDto>> GetScheduleAsync(DateTime startDate, DateTime endDate, int? employeeId = null);
        Task<bool> DeleteAssignmentAsync(int id);
    }
}
