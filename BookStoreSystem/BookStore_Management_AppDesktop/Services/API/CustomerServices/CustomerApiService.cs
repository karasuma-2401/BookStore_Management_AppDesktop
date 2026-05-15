using BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.CustomerServices
{
    public class CustomerApiService : ICustomerApiService
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
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<List<CustomerResponseDto>?> GetAllCustomersAsync()
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync("customer");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<CustomerResponseDto>>(json, _options) ?? new List<CustomerResponseDto>();
                }
                return new List<CustomerResponseDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllCustomers Error: {ex.Message}");
                return null;
            }
        }

        public async Task<CustomerResponseDto?> GetCustomerByIdAsync(int id)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync($"customer/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<CustomerResponseDto>(json, _options);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetCustomerById Error: {ex.Message}");
                return null;
            }
        }

        public async Task<CustomerResponseDto?> CreateCustomerAsync(CustomerCreateDto customer)
        {
            try
            {
                AddAuthorizationHeader();

                var json = JsonSerializer.Serialize(customer, _options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("customer", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<CustomerResponseDto>(responseJson, _options);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateCustomer Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateCustomerAsync(int id, CustomerUpdateDto customer)
        {
            try
            {
                AddAuthorizationHeader();

                var json = JsonSerializer.Serialize(customer, _options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"customer/{id}", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateCustomer Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"customer/{id}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteCustomer Error: {ex.Message}");
                return false;
            }
        }
    }
}