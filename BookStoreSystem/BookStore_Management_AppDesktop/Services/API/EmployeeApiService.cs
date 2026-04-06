using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.Services.API
{
    public class EmployeeApiService : IEmployeeApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7063/") };
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private void AddAuthorizationHeader()
        {
            // Retrieve the token saved after a successful login
            var token = Settings.Default.AccessToken;

            if (!string.IsNullOrEmpty(token))
            {
                // Assign Bearer Token to the Header
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync("employee");

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("You do not have Admin permissions to view this list!", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return new List<Employee>();
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error: {response.StatusCode} - {error}");
                    return new List<Employee>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var dtos = JsonSerializer.Deserialize<List<EmployeeResponseDto>>(json, _options) ?? new List<EmployeeResponseDto>();

                return dtos.Select(dto => new Employee
                {
                    EmployeeId = dto.EmployeeId,
                    FullName = dto.FullName,
                    Age = dto.Age,
                    Phone = dto.Phone,
                    Address = dto.Address,
                    Salary = dto.Salary,
                    UserId = dto.UserId
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection Error: {ex.Message}", "Network Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Employee>();
            }
        }

        public async Task<bool> CreateEmployeeAsync(Employee emp)
        {
            try
            {
                AddAuthorizationHeader();
                var dto = new EmployeeCreateDto
                {
                    FullName = emp.FullName,
                    Age = emp.Age,
                    Phone = emp.Phone,
                    Address = emp.Address,
                    Salary = emp.Salary,
                    UserId = emp.UserId
                };

                var json = JsonSerializer.Serialize(dto, _options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("employee", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateEmployee Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(int id, Employee emp)
        {
            try
            {
                AddAuthorizationHeader();
                var dto = new EmployeeResponseDto
                {
                    EmployeeId = id,
                    FullName = emp.FullName,
                    Age = emp.Age,
                    Phone = emp.Phone,
                    Address = emp.Address,
                    Salary = emp.Salary,
                    UserId = emp.UserId
                };

                var json = JsonSerializer.Serialize(dto, _options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"employee/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateEmployee Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"employee/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteEmployee Error: {ex.Message}");
                return false;
            }
        }
    }
}