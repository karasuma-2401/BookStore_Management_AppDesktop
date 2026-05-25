using BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs;
using BookStore_Management_AppDesktop.Services.API.InvoiceServices;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows;

namespace BookStore_Management_AppDesktop.Services.API
{
    public class InvoiceApiService : IInvoiceApiService
    {
        private static readonly HttpClient _httpClient =
            new HttpClient { BaseAddress = new Uri("https://localhost:7063/") };

        private void AddAuthorizationHeader()
        {
            var token = Settings.Default.AccessToken;

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<List<InvoiceListDto>> GetAllInvoicesAsync()
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync("invoice");

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    MessageBox.Show(
                        "You do not have Admin permissions!",
                        "Access Denied",
                        MessageBoxButton.OK,
                        MessageBoxImage.Stop);

                    return new List<InvoiceListDto>();
                }

                if (!response.IsSuccessStatusCode)
                    return new List<InvoiceListDto>();

                return await response.Content.ReadFromJsonAsync<List<InvoiceListDto>>()
                       ?? new List<InvoiceListDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAllInvoices Error: {ex.Message}");
                return new List<InvoiceListDto>();
            }
        }

        public async Task<InvoiceDetailResponseDto?> GetInvoiceByIdAsync(int id)
        {
            try
            {
                // 1. Thêm Token Bearer vào Header trước khi gọi
                AddAuthorizationHeader();

                // 2. Gọi API lấy dữ liệu thô về trước để kiểm tra trạng thái HTTP
                var response = await _httpClient.GetAsync($"invoice/{id}");

                // Nếu Backend trả về lỗi (401, 403, 404, 500...)
                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ API Invoice Detail Error! Mã lỗi HTTP: {response.StatusCode}");

                    // Hiện thông báo trực quan để bạn dễ debug khi test
                    MessageBox.Show($"Không thể tải chi tiết hóa đơn. Mã lỗi HTTP: {response.StatusCode}",
                                    "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                // 3. Cấu hình JsonSerializer để CHẤP NHẬN chữ hoa/thường (camelCase từ API sang PascalCase của C#)
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // 4. Ép kiểu chuỗi JSON sang Object C# với cấu hình options ở trên
                return await response.Content.ReadFromJsonAsync<InvoiceDetailResponseDto>(options);
            }
            catch (Exception ex)
            {
                // Bắt các lỗi crash bất ngờ (lỗi kết nối mạng, lỗi ép kiểu nghiêm trọng)
                System.Diagnostics.Debug.WriteLine($"❌ GetInvoiceById Crash: {ex.Message}");
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public async Task<bool> CancelInvoiceAsync(int id)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.PatchAsync($"invoice/{id}/cancel", null);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CancelInvoice Error: {ex.Message}");
                return false;
            }
        }

        public async Task<int?> CreateInvoiceAsync(InvoiceCreateDto invoiceCreateDto)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.PostAsJsonAsync("invoice/checkout", invoiceCreateDto);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"CreateInvoice Error: HTTP {response.StatusCode}");
                    return null;
                }

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = await response.Content.ReadFromJsonAsync<CreateInvoiceResponse>(options);
                return result?.InvoiceId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateInvoice Error: {ex.Message}");
                return null;
            }
        }

        private class CreateInvoiceResponse
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public int? InvoiceId { get; set; }
        }

    }
}