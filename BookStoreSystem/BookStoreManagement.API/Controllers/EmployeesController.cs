using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Interfaces.Services;
using BookStoreManagement.API.Models.Employee;

namespace BookStoreManagement.API.Controllers
{
    [Route("employee")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            return employee == null
                ? NotFound(new { message = "Employee not found." })
                : Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> PostEmployee(EmployeeCreateDto employee)
        {
            var success = await _employeeService.CreateEmployeeAsync(employee);
            return success
                ? StatusCode(201, new { message = "Employee added successfully" })
                : BadRequest(new { message = "Add failed" });
        }

        [HttpPut("{id}")]   
        public async Task<IActionResult> PutEmployee(int id, EmployeeUpdateDto employee)
        {
            var success = await _employeeService.UpdateEmployeeAsync(id, employee);
            return success
                ? NoContent()
                : BadRequest(new { message = "Update failed or ID does not match" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var success = await _employeeService.DeleteEmployeeAsync(id);
            return success
                ? NoContent()
                : NotFound(new { message = "Employee not found." });
        }


    }
}
