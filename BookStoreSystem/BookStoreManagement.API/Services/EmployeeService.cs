using BookStoreManagement.API.Data;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Employee;
using BookStoreManagement.API.Models.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDBContext _context;
        private readonly IValidator<Employee> _validator;
        
        public EmployeeService(ApplicationDBContext context, IValidator<Employee> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllEmployeesAsync()
        {
            return await _context.Employees.
                Select(e => new EmployeeResponseDto
                {
                    EmployeeId = e.EmployeeId,
                    UserId = e.UserId,
                    FullName = e.FullName,
                    Age = e.Age,
                    Phone = e.Phone,
                    Address = e.Address,
                    Salary = e.Salary
                })
                .ToListAsync();
        }

        public async Task<EmployeeResponseDto?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
            .Where(e => e.EmployeeId == id)
            .Select(e => new EmployeeResponseDto
            {
                EmployeeId = e.EmployeeId,
                UserId = e.UserId,
                FullName = e.FullName,
                Age = e.Age,
                Phone = e.Phone,
                Address = e.Address,
                Salary = e.Salary
            })
            .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateEmployeeAsync(EmployeeCreateDto dto)
        {
            var employee = new Employee
            {
                UserId = dto.UserId,
                FullName = dto.FullName,
                Age = dto.Age,
                Phone = dto.Phone,
                Address = dto.Address,
                Salary = dto.Salary
            };

            var validationResult = await _validator.ValidateAsync(employee);
            if (!validationResult.IsValid) return false;

            _context.Employees.Add(employee);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;

            employee.FullName = dto.FullName;
            employee.Age = dto.Age;
            employee.Phone = dto.Phone;
            employee.Address = dto.Address;
            employee.Salary = dto.Salary;

            var validationResult = await _validator.ValidateAsync(employee);
            if (!validationResult.IsValid) return false;

            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;
            _context.Employees.Remove(employee);
            return await _context.SaveChangesAsync() > 0;
        }



    }
}
