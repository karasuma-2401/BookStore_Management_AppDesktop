using BookStore_Management_AppDesktop.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API
{
    public class UserApiService : IUserApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7063/") };
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private void AddAuthorizationHeader()
        {
            var token = Settings.Default.AccessToken; // Lấy token từ Settings
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<UserResponseModel>> GetAllUsersAsync()
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync("user");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<UserResponseModel>>(json, _options) ?? new List<UserResponseModel>();
                }
                return new List<UserResponseModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi UserApi: {ex.Message}");
                return new List<UserResponseModel>();
            }
        }
    }
}