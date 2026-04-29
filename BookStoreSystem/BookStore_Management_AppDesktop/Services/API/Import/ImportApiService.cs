using BookStore_Management_AppDesktop.Models.DTOs.ImportDTOs;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.Import
{
    public class ImportApiService : IImportApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7063/")
        };

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private void AddAuthorizationHeader()
        {
            var token = Settings.Default.AccessToken;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> CreateImportAsync(ImportCreateDTO dto)
        {
            try
            {
                AddAuthorizationHeader();

                var json = JsonSerializer.Serialize(dto, _options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("import", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateImport Error: {ex.Message}");
                return false;
            }
        }
        public async Task<List<ImportResponseDto>> GetAllImportsAsync()
        {
            try
            {
                AddAuthorizationHeader(); 

                var response = await _httpClient.GetAsync("import");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<ImportResponseDto>>(json, _options) ?? new List<ImportResponseDto>();
                }
                return new List<ImportResponseDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllImports Error: {ex.Message}");
                return new List<ImportResponseDto>();
            }
        }
    }
}

