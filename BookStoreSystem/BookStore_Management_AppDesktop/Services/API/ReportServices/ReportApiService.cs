using BookStore_Management_AppDesktop.Models.DTOs.ReportDTOs;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.ReportServices
{
    public class ReportApiService : IReportApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7063/") };
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private void AddAuthorizationHeader()
        {
            var token = Settings.Default.AccessToken;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<ReportSummaryDto?> GetMonthlyReportAsync(int month, int year)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/report/monthly?month={month}&year={year}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ReportSummaryDto>(json, _options);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Get Report Error: {ex.Message}");
                return null;
            }
        }
    }
}