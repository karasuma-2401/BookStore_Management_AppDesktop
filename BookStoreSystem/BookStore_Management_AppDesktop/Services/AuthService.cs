using BookStore_Management_AppDesktop.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7063/"); 
        }

        public async Task<LoginResponseModel?> LoginAsync(string username, string password)
        {
            try
            {
                var requestData = new LoginRequestModel
                {
                    Username = username,
                    Password = password
                };
                var response = await _httpClient.PostAsJsonAsync("auth/login", requestData);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponseModel>();
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error call API Auth: {ex.Message}");
                return null;
            }
        }
    }
}