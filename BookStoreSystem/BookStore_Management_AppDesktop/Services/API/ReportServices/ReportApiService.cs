using BookStore_Management_AppDesktop.Models.DTOs.ReportDTOs;
using System;
using System.Collections.Generic;
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
                var response = await _httpClient.GetAsync($"report/monthly?month={month}&year={year}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ReportSummaryDto>(json, _options);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error for call API: Code {response.StatusCode} - Content: {error}");
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Get Monthly Report Error: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<InventoryReportResponseDTO>> GetInventoryReportsAsync(int month, int year)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync($"inventory-report?month={month}&year={year}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<IEnumerable<InventoryReportResponseDTO>>(json, _options) ?? new List<InventoryReportResponseDTO>();
                }
                return new List<InventoryReportResponseDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Get Inventory Report Error: {ex.Message}");
                return new List<InventoryReportResponseDTO>();
            }
        }

        public async Task<IEnumerable<DebtReportResponseDTO>> GetDebtReportsAsync(int month, int year)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync($"debt-report?month={month}&year={year}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<IEnumerable<DebtReportResponseDTO>>(json, _options) ?? new List<DebtReportResponseDTO>();
                }
                return new List<DebtReportResponseDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Get Debt Report Error: {ex.Message}");
                return new List<DebtReportResponseDTO>();
            }
        }
    }
}