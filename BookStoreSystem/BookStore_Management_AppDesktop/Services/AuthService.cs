using BookStore_Management_AppDesktop.Models.Dtos;   
using System;
using System.Diagnostics;
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
        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var payload = new {email = email};
                var response = await _httpClient.PostAsJsonAsync("user/forgot-password", payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine ($"Errors for calling api: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> ResetPasswordAsync (string token, string newPassword, string confirmPassword)
        {
            try
            {
                var payload = new
                {
                    token = token,
                    newPassword = newPassword,
                    confirmPassword = confirmPassword
                };
                var response = await _httpClient.PostAsJsonAsync("user/reset-password", payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine ($"Errors to calling api: {ex.Message}");
                return false;
            }
        }
    }
}