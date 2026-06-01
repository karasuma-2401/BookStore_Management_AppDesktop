using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.CategoryDTOs; 
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.CategoryServices
{
    public class CategoryApiService : ICategoryApiService
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

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync("category");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var categories = JsonSerializer.Deserialize<List<Category>>(json, _options);
                    return categories ?? new List<Category>();
                }

                System.Diagnostics.Debug.WriteLine($"[API Fail] GetAllCategories: {response.StatusCode}");
                return new List<Category>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CRASH] GetAllCategories Exception: {ex.Message}");
                return new List<Category>();
            }
        }

        public async Task<Category?> CreateCategoryAsync(string name)
        {
            try
            {
                AddAuthorizationHeader();

                var dto = new CategoryCreateDto { Name = name };
                var json = JsonSerializer.Serialize(dto, _options);

                var content = new StringContent(json);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json")
                {
                    CharSet = "utf-8"
                };

                var response = await _httpClient.PostAsync("category", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Category>(responseJson, _options);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CRASH] CreateCategory Exception: {ex.Message}");
                return null;
            }
        }
    }
}