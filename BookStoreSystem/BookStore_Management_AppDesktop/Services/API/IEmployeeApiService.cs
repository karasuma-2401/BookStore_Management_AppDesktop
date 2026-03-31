using BookStore_Management_AppDesktop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API
{
    public interface IEmployeeApiService
    {
        Task<List<Employee>> GetAllEmployeesAsync();
        Task<bool> CreateEmployeeAsync(Employee newEmployee);
        Task<bool> UpdateEmployeeAsync(int id, Employee updatedEmployee);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}