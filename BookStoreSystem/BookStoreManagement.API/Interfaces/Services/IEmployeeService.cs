using BookStoreManagement.API.Models.Employee;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeResponseDto>> GetAllEmployeesAsync();
        Task<EmployeeResponseDto?> GetEmployeeByIdAsync(int id);
        Task<bool> CreateEmployeeAsync(EmployeeCreateDto employee);
        Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateDto employee);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
