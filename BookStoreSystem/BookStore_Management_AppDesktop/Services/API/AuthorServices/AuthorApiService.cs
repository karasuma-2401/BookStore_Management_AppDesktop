using BookStore_Management_AppDesktop.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.AuthorServices
{
    public class AuthorApiService : IAuthorApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7063/") };

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private void AddAuthorizationHeader()
        {
            var token = Settings.Default.AccessToken;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync("author");
                if (!response.IsSuccessStatusCode) return new List<Author>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Author>>(json, _options) ?? new List<Author>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllAuthors Error: {ex.Message}");
                return new List<Author>();
            }
        }

        public async Task<Author?> CreateAuthorAsync(string name)
        {
            try
            {
                AddAuthorizationHeader();
                var createDto = new { Name = name };
                var json = JsonSerializer.Serialize(createDto, _options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("author", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Author>(responseJson, _options);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateAuthor Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateAuthorAsync(int id, string name)
        {
            try
            {
                AddAuthorizationHeader();

                var updateDto = new { Name = name };
                var json = JsonSerializer.Serialize(updateDto, _options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"author/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateAuthor Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"author/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteAuthor Error: {ex.Message}");
                return false;
            }
        }
    }
}