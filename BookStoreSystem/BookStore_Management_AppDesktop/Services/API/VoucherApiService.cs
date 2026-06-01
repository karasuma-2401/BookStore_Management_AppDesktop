using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Diagnostics;

namespace BookStore_Management_AppDesktop.Services.API
{
    public interface IVoucherApiService
    {
        Task<List<VoucherDto>> GetAllVouchersAsync();
        Task<VoucherDto?> CreateVoucherAsync(VoucherCreateRequestDto dto);
        Task<(bool Success, string Message)> DeleteVoucherAsync(int id);
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

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<List<VoucherDto>>(options)
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
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"CreateVoucher Error: HTTP {response.StatusCode}. Details: {errorContent}");
                    throw new Exception($"Failed to create voucher: {errorContent}");
                }

                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await response.Content.ReadFromJsonAsync<CreateVoucherResponse>(options);
                return result?.Data;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"CreateVoucher HttpError: {ex.Message}");
                throw new Exception($"Network error: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"CreateVoucher JsonError: {ex.Message}");
                throw new Exception($"Invalid response format: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateVoucher Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<(bool Success, string Message)> DeleteVoucherAsync(int id)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.DeleteAsync($"voucher/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string message = "Voucher deleted successfully.";
                    try
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var successObj = await response.Content.ReadFromJsonAsync<JsonElement>(options);
                        if (successObj.ValueKind == JsonValueKind.Object && successObj.TryGetProperty("message", out var msgProp))
                        {
                            message = msgProp.GetString() ?? message;
                        }
                    }
                    catch
                    {
                        // Ignore JSON parsing errors for success
                    }
                    return (true, message);
                }
                else
                {
                    string errorMessage = "Failed to delete voucher";
                    try
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>(options);
                        if (errorObj.ValueKind == JsonValueKind.Object && errorObj.TryGetProperty("message", out var msgProp))
                        {
                            errorMessage = msgProp.GetString() ?? errorMessage;
                        }
                    }
                    catch
                    {
                        errorMessage = response.ReasonPhrase ?? errorMessage;
                    }
                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteVoucher Error: {ex.Message}");
                return (false, ex.Message);
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
