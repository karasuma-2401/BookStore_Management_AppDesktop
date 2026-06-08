using BookStore_Management_AppDesktop.Models.DTOs.RegulationDTOs;
using BookStore_Management_AppDesktop.Services;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace BookStore_Management_AppDesktop.Services
{
    public class RegulationApiService : IRegulationApiService
    {
        private readonly HttpClient _http;

        public RegulationApiService(HttpClient http)
        {
            _http = http;
            Debug.WriteLine($"[RegulationApiService] Initialized with BaseAddress: {_http.BaseAddress}");
        }

        /// <summary>
        /// Adds JWT token to request headers for authentication
        /// </summary>
        private void AddAuthorizationHeader()
        {
            var token = Settings.Default.AccessToken;
            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Debug.WriteLine("[RegulationApiService] Authorization header added");
            }
            else
            {
                Debug.WriteLine("[RegulationApiService] WARNING: No access token found");
            }
        }

        public async Task<List<RegulationResponseDto>> GetAllAsync()
        {
            try
            {
                AddAuthorizationHeader();
                Debug.WriteLine($"[RegulationApiService] Requesting: GET {_http.BaseAddress}setting");

                var result = await _http.GetFromJsonAsync<List<RegulationResponseDto>>("setting");

                Debug.WriteLine($"[RegulationApiService] Success! Received {result?.Count ?? 0} items");

                return result ?? new List<RegulationResponseDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RegulationApiService] ERROR: {ex.GetType().Name} - {ex.Message}");
                Debug.WriteLine($"[RegulationApiService] Stack: {ex.StackTrace}");
                throw;
            }
        }


        public async Task<RegulationResponseDto> GetByNameAsync(string name)
        {
            AddAuthorizationHeader();
            Debug.WriteLine($"[RegulationApiService] Requesting: GET {_http.BaseAddress}setting/{name}");
            return await _http.GetFromJsonAsync<RegulationResponseDto>($"setting/{name}");
        }

        public async Task<bool> CreateAsync(RegulationCreateDto dto)
        {
            AddAuthorizationHeader();
            Debug.WriteLine($"[RegulationApiService] Requesting: POST setting?name={dto.SettingName}&value={dto.Value}");

            var response = await _http.PostAsync(
                $"setting?name={dto.SettingName}&value={dto.Value}", null);

            Debug.WriteLine($"[RegulationApiService] Create Response: {response.StatusCode}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(string name, RegulationUpdateDto dto)
        {
            AddAuthorizationHeader();
            Debug.WriteLine($"[RegulationApiService] Requesting: PUT setting/{name}");

            var response = await _http.PutAsJsonAsync($"setting/{name}", dto);

            Debug.WriteLine($"[RegulationApiService] Update Response: {response.StatusCode}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string name)
        {
            AddAuthorizationHeader();
            Debug.WriteLine($"[RegulationApiService] Requesting: DELETE setting/{name}");

            var response = await _http.DeleteAsync($"setting/{name}");

            Debug.WriteLine($"[RegulationApiService] Delete Response: {response.StatusCode}");
            return response.IsSuccessStatusCode;
        }
    }
}