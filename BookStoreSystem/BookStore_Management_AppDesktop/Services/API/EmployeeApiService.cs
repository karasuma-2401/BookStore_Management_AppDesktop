using BookStore_Management_AppDesktop.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API
{
    public class EmployeeApiService : IEmployeeApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7063/api/") // Đảm bảo Port này đúng với API đang chạy
        };

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("employees");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Employee>>(json, _options) ?? new List<Employee>();
                }
                return new List<Employee>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi kéo data nhân viên: {ex.Message}");
                return new List<Employee>();
            }
        }
    }
}