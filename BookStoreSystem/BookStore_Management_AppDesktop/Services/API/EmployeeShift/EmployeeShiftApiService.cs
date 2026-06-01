using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Diagnostics;

using System.Net.Http;
using BookStore_Management_AppDesktop.Models.DTOs.ShiftDTOs;


namespace BookStore_Management_AppDesktop.Services.API
{
    public class EmployeeShiftApiService : IEmployeeShiftApiService
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

        public async Task<List<EmployeeShiftResponseDto>> GetScheduleAsync(DateTime startDate, DateTime endDate, int? employeeId = null)
        {
            try
            {
                AddAuthorizationHeader();

                var query = $"employeeshift/schedule?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
                if (employeeId.HasValue)
                    query += $"&employeeId={employeeId.Value}";

                var response = await _httpClient.GetAsync(query);

                if (!response.IsSuccessStatusCode)
                    return new List<EmployeeShiftResponseDto>();

                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<List<EmployeeShiftResponseDto>>(options)
                       ?? new List<EmployeeShiftResponseDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetSchedule Error: {ex.Message}");
                return new List<EmployeeShiftResponseDto>();
            }
        }

        public async Task<List<EmployeeShiftResponseDto>> GetAbsentShiftsAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var schedule = await GetScheduleAsync(startDate, endDate);
                return schedule.Where(s => s.Status.Equals("Absent", StringComparison.OrdinalIgnoreCase)).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAbsentShifts Error: {ex.Message}");
                return new List<EmployeeShiftResponseDto>();
            }
        }

        public async Task<(bool Success, string Message)> AssignShiftAsync(ShiftAssignDto dto)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.PostAsJsonAsync("employeeshift/assign", dto);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Shift assigned successfully!");
                }
                else
                {
                    string errorMessage = "Failed to assign shift. The employee might already have a shift today.";
                    try
                    {
                        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var errorObj = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>(options);
                        if (errorObj.ValueKind == System.Text.Json.JsonValueKind.Object)
                        {
                            if (errorObj.TryGetProperty("message", out var msgProp))
                            {
                                errorMessage = msgProp.GetString() ?? errorMessage;
                            }
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
                Debug.WriteLine($"AssignShift Error: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.DeleteAsync($"employeeshift/assignment/{id}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteAssignment Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ApproveCompensationAsync(int id)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.PutAsync($"employeeshift/compensate/{id}", null);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ApproveCompensation Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ApproveBulkCompensationAsync(List<int> assignmentIds)
        {
            try
            {
                AddAuthorizationHeader();

                var tasks = assignmentIds.Select(id => _httpClient.PutAsync($"employeeshift/compensate/{id}", null));
                var responses = await Task.WhenAll(tasks);

                return responses.All(r => r.IsSuccessStatusCode);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ApproveBulkCompensation Error: {ex.Message}");
                return false;
            }
        }

        public async Task<PayslipDto?> GetPayrollAsync(int employeeId, int month, int year)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync($"employeeshift/payroll/{employeeId}?month={month}&year={year}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<PayslipDto>(options);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetPayroll Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ShiftDayDetailResponseDto?> GetDayDetailAsync(DateTime date)
        {
            try
            {
                AddAuthorizationHeader();

                var query = $"employeeshift/day-detail?date={date:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync(query);

                if (!response.IsSuccessStatusCode)
                    return null;

                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<ShiftDayDetailResponseDto>(options);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetDayDetail Error: {ex.Message}");
                return null;
            }
        }

        public async Task<KioskCheckInResponseDto?> KioskCheckInAsync(int employeeId)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.PostAsJsonAsync("employeeshift/kiosk-checkin", new { EmployeeId = employeeId });

                if (!response.IsSuccessStatusCode)
                {
                    try
                    {
                        var errorResult = await response.Content.ReadFromJsonAsync<KioskCheckInResponseDto>();
                        if (errorResult != null) return errorResult;
                    }
                    catch { }
                    return new KioskCheckInResponseDto { Success = false, Message = $"Server returned error {response.StatusCode}." };
                }

                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<KioskCheckInResponseDto>(options);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"KioskCheckIn Error: {ex.Message}");
                return new KioskCheckInResponseDto { Success = false, Message = $"Connection error: {ex.Message}" };
            }
        }
    }
}
