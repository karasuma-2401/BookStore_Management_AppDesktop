using BookStore_Management_AppDesktop.Models.DTOs.ShiftDTOs;
using BookStore_Management_AppDesktop.Services.API;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class KioskCheckInViewModel : ObservableObject
    {
        private readonly IEmployeeShiftApiService _shiftApiService;
        private readonly DispatcherTimer _clockTimer;

        [ObservableProperty]
        private string _employeeIdText = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private string _successMessage = string.Empty;

        [ObservableProperty]
        private string _employeeName = string.Empty;

        [ObservableProperty]
        private string _shiftName = string.Empty;

        [ObservableProperty]
        private string _workTime = string.Empty;

        [ObservableProperty]
        private string _checkInTimeText = string.Empty;

        [ObservableProperty]
        private string _status = string.Empty;

        [ObservableProperty]
        private bool _showFeedback = false;

        [ObservableProperty]
        private string _currentTimeText = string.Empty;

        public KioskCheckInViewModel(IEmployeeShiftApiService shiftApiService)
        {
            _shiftApiService = shiftApiService;

            // Cập nhật đồng hồ thời gian thực
            _clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _clockTimer.Tick += ClockTimer_Tick;
            _clockTimer.Start();
            UpdateClockText();
        }

        private void ClockTimer_Tick(object? sender, EventArgs e)
        {
            UpdateClockText();
        }

        private void UpdateClockText()
        {
            CurrentTimeText = DateTime.Now.ToString("HH:mm:ss - dddd, dd/MM/yyyy");
        }

        [RelayCommand]
        private async Task CheckInAsync()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            ShowFeedback = false;

            if (string.IsNullOrWhiteSpace(EmployeeIdText))
            {
                ErrorMessage = "Please enter your Employee ID.";
                return;
            }

            if (!int.TryParse(EmployeeIdText, out int employeeId))
            {
                ErrorMessage = "Employee ID must be a valid number.";
                return;
            }

            IsLoading = true;
            try
            {
                var result = await _shiftApiService.KioskCheckInAsync(employeeId);

                if (result == null)
                {
                    ErrorMessage = "Failed to communicate with API server.";
                    return;
                }

                if (result.Success)
                {
                    SuccessMessage = result.Message;
                    EmployeeName = result.EmployeeName;
                    ShiftName = result.ShiftName;
                    WorkTime = result.WorkTime;
                    CheckInTimeText = result.CheckInTime.HasValue 
                        ? result.CheckInTime.Value.ToLocalTime().ToString("HH:mm:ss dd/MM/yyyy") 
                        : DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
                    Status = result.Status;
                    ShowFeedback = true;

                    // Tự động xóa input để nhân viên sau điểm danh
                    EmployeeIdText = string.Empty;

                    // Giữ phản hồi hiển thị trong 7 giây rồi ẩn đi
                    await Task.Delay(7000);
                    if (ShowFeedback && string.IsNullOrEmpty(ErrorMessage))
                    {
                        ShowFeedback = false;
                    }
                }
                else
                {
                    ErrorMessage = result.Message;
                    EmployeeName = result.EmployeeName;
                    ShiftName = result.ShiftName;
                    WorkTime = result.WorkTime;
                    Status = result.Status;
                    
                    if (!string.IsNullOrEmpty(EmployeeName))
                    {
                        // Hiển thị phản hồi lỗi có tên nhân viên (ví dụ: đã điểm danh rồi hoặc quá sớm)
                        CheckInTimeText = result.CheckInTime.HasValue 
                            ? result.CheckInTime.Value.ToLocalTime().ToString("HH:mm:ss dd/MM/yyyy") 
                            : string.Empty;
                        ShowFeedback = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CheckInCommand Error: {ex.Message}");
                ErrorMessage = "An unexpected error occurred.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void ClearFeedback()
        {
            ShowFeedback = false;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }
    }
}
