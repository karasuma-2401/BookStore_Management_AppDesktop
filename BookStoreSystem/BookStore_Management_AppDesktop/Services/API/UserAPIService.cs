using BookStore_Management_AppDesktop.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            var token = Settings.Default.AccessToken;
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
                System.Diagnostics.Debug.WriteLine($"UserApi Error: {ex.Message}");
                return new List<UserResponseModel>();
            }
        }

        // Updated to return both success status and the error message string
        public async Task<(bool IsSuccess, string Message)> CreateUserAsync(UserCreateDto dto)
        {
            try
            {
                AddAuthorizationHeader();

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("user", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "User created successfully!");
                }

                // Try to extract the "message" field from the Backend's JSON response
                try
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    if (doc.RootElement.TryGetProperty("message", out var messageElement))
                    {
                        return (false, messageElement.GetString() ?? "Invalid data.");
                    }
                }
                catch
                {
                    // Fallback if the response is not valid JSON
                    return (false, $"Error: {response.StatusCode}");
                }

                return (false, "Creation failed.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateUserApi Error: {ex.Message}");
                return (false, $"System Error: {ex.Message}");
            }
        }
    }
}