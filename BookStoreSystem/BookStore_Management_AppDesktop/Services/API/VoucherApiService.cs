using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BookStore_Management_AppDesktop.Services.API
{
    public interface IVoucherApiService
    {
        Task<List<VoucherDto>> GetAllVouchersAsync();
        Task<VoucherDto?> CreateVoucherAsync(VoucherCreateRequestDto dto);
        Task<bool> DeleteVoucherAsync(int id);
    }

    public class VoucherApiService : IVoucherApiService
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

        public async Task<List<VoucherDto>> GetAllVouchersAsync()
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync("voucher");

                if (!response.IsSuccessStatusCode)
                    return new List<VoucherDto>();

                return await response.Content.ReadFromJsonAsync<List<VoucherDto>>()
                       ?? new List<VoucherDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAllVouchers Error: {ex.Message}");
                return new List<VoucherDto>();
            }
        }

        public async Task<VoucherDto?> CreateVoucherAsync(VoucherCreateRequestDto dto)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync("voucher", dto);

                if (!response.IsSuccessStatusCode)
                {
                    // ĐỌC NỘI DUNG LỖI TỪ BACKEND
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"CreateVoucher Error: HTTP {response.StatusCode}. Details: {errorContent}");

                    // Nếu bạn muốn hiển thị lỗi này lên UI, bạn có thể ném exception kèm message
                    throw new Exception($"Server error: {errorContent}");
                }

                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await response.Content.ReadFromJsonAsync<CreateVoucherResponse>(options);
                return result?.Data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateVoucher Exception: {ex.Message}");
                throw; // Ném lại lỗi để ViewModel bắt được
            }
        }

        public async Task<bool> DeleteVoucherAsync(int id)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.DeleteAsync($"voucher/{id}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteVoucher Error: {ex.Message}");
                return false;
            }
        }

        private class CreateVoucherResponse
        {
            public string? Message { get; set; }
            public VoucherDto? Data { get; set; }
        }
    }

    public class VoucherDto
    {
        public int VoucherId { get; set; }
        public string Code { get; set; } = null!;
        public int? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
    }

    public class VoucherCreateRequestDto
    {
        public string Code { get; set; } = null!;
        public int? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? UsageLimit { get; set; }
    }
}
