using BookStore_Management_AppDesktop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API
{
    public interface IEmployeeApiService
    {
        Task<List<Employee>> GetAllEmployeesAsync();
    }
}