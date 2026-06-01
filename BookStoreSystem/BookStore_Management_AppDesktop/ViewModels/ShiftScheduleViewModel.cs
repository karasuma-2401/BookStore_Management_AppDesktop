using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.EmployeeServices;
using BookStore_Management_AppDesktop.Models.DTOs.ShiftDTOs;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services.Export;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;


namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class ShiftScheduleViewModel : ObservableObject
    {
        private readonly IEmployeeShiftApiService _shiftApiService;
        private readonly IEmployeeApiService _employeeApiService;
        private readonly IDialogService _dialogService;
        private readonly IExportService _exportService;

        [ObservableProperty]
        private ObservableCollection<ScheduleCellViewModel> scheduleCells = new();

        [ObservableProperty]
        private DateTime currentMonthStart;

        [ObservableProperty]
        private ObservableCollection<EmployeeShiftResponseDto> allShifts = new();

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private string loadingMessage = "Loading...";

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Now;

        [ObservableProperty]
        private string? selectedStatusFilter = null;

        [ObservableProperty]
        private string? selectedShiftFilter = null;

        [ObservableProperty]
        private ObservableCollection<string> availableStatuses = new();

        [ObservableProperty]
        private ObservableCollection<string> availableShifts = new();

        private List<Employee> _employees = new();
        private List<EmployeeShiftResponseDto> _unfilteredShifts = new();

        public ShiftScheduleViewModel(IEmployeeShiftApiService shiftApiService, 
                                     IEmployeeApiService employeeApiService,
                                     IDialogService dialogService,
                                     IExportService exportService)
        {
            _shiftApiService = shiftApiService;
            _employeeApiService = employeeApiService;
            _dialogService = dialogService;
            _exportService = exportService;

            CurrentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading employees...";

                var employees = await _employeeApiService.GetAllEmployeesAsync();
                _employees = employees ?? new List<Employee>();

                await LoadScheduleAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Initialize Error: {ex.Message}");
                _dialogService.ShowMessage($"Failed to initialize: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadScheduleAsync()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading schedule...";

                var startDate = CurrentMonthStart;
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var shifts = await _shiftApiService.GetScheduleAsync(startDate, endDate);
                _unfilteredShifts = shifts.ToList();

                // Update available filters
                UpdateFilterOptions();

                // Apply filters
                ApplyFilters();

                GenerateCalendarGrid();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadSchedule Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateFilterOptions()
        {
            var statuses = _unfilteredShifts.Select(s => s.Status).Distinct().OrderBy(s => s).ToList();
            AvailableStatuses.Clear();
            foreach (var status in statuses)
            {
                AvailableStatuses.Add(status);
            }

            var shifts = _unfilteredShifts.Select(s => s.ShiftName).Distinct().OrderBy(s => s).ToList();
            AvailableShifts.Clear();
            foreach (var shift in shifts)
            {
                AvailableShifts.Add(shift);
            }
        }

        private void ApplyFilters()
        {
            var filtered = _unfilteredShifts.AsEnumerable();

            if (!string.IsNullOrEmpty(SelectedStatusFilter))
            {
                filtered = filtered.Where(s => s.Status == SelectedStatusFilter);
            }

            if (!string.IsNullOrEmpty(SelectedShiftFilter))
            {
                filtered = filtered.Where(s => s.ShiftName == SelectedShiftFilter);
            }

            AllShifts.Clear();
            foreach (var shift in filtered)
            {
                AllShifts.Add(shift);
            }
        }

        private void GenerateCalendarGrid()
        {
            ScheduleCells.Clear();

            var startDate = CurrentMonthStart;
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var daysInMonth = endDate.Day;

            // Tính toán số ngày trống cần đệm ở đầu tháng (Thứ Hai = 0, ..., Chủ Nhật = 6)
            int firstDayOffset = ((int)startDate.DayOfWeek + 6) % 7;

            // Thêm các ô trống đệm vào trước ngày đầu tiên của tháng
            for (int i = 0; i < firstDayOffset; i++)
            {
                ScheduleCells.Add(new ScheduleCellViewModel
                {
                    IsDummy = true,
                    Date = null,
                    DayOfWeek = string.Empty,
                    DayNumber = 0
                });
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var dateCell = new DateTime(startDate.Year, startDate.Month, day);
                var shiftsForDay = AllShifts
                    .Where(s =>
                    {
                        if (string.IsNullOrWhiteSpace(s.WorkDate)) return false;

                        // Backend trả WorkDate dạng yyyy-MM-dd, tránh DateTime.Parse phụ thuộc culture máy chạy
                        if (DateTime.TryParseExact(
                                s.WorkDate,
                                "yyyy-MM-dd",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out var workDate))
                        {
                            return workDate.Date == dateCell.Date;
                        }

                        // Fallback: nếu format khác thì bỏ qua (để tránh UI trống do parse sai)
                        return false;
                    })
                    .ToList();


                var cellViewModel = new ScheduleCellViewModel
                {
                    Date = dateCell,
                    Shifts = new ObservableCollection<EmployeeShiftResponseDto>(shiftsForDay),
                    DayOfWeek = dateCell.ToString("ddd"),
                    DayNumber = day,
                    IsDummy = false
                };

                ScheduleCells.Add(cellViewModel);
            }
        }

        [RelayCommand]
        private async Task PreviousMonth()
        {
            CurrentMonthStart = CurrentMonthStart.AddMonths(-1);
            await LoadScheduleAsync();
        }

        [RelayCommand]
        private async Task NextMonth()
        {
            CurrentMonthStart = CurrentMonthStart.AddMonths(1);
            await LoadScheduleAsync();
        }

        [RelayCommand]
        private void ClearFilters()
        {
            SelectedStatusFilter = null;
            SelectedShiftFilter = null;
            ApplyFilters();
            GenerateCalendarGrid();
        }

        [RelayCommand]
        private void UpdateFilters()
        {
            ApplyFilters();
            GenerateCalendarGrid();
        }

        [RelayCommand]
        private async Task AssignShift(DateTime date)
        {
            if (IsLoading) return;
            await OpenAssignShiftDialog(date);
        }


        [RelayCommand]
        private async Task DeleteShift(EmployeeShiftResponseDto shift)
        {
            if (shift == null) return;

            var confirmed = _dialogService.ShowConfirmation(
                $"Are you sure you want to delete the shift assignment for {shift.FullName} on {shift.WorkDate}?",
                "Delete",
                true);

            if (!confirmed) return;

            try
            {
                IsLoading = true;
                var success = await _shiftApiService.DeleteAssignmentAsync(shift.Id);

                if (success)
                {
                    _dialogService.ShowMessage("Shift deleted successfully!");
                    await LoadScheduleAsync();
                }
                else
                {
                    _dialogService.ShowMessage("Failed to delete shift.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteShift Error: {ex.Message}");
                _dialogService.ShowMessage($"Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public List<Employee> GetEmployees() => _employees;

        public async Task<bool> AssignShiftToDate(int employeeId, int shiftId, DateTime workDate)
        {
            try
            {
                IsLoading = true;
                var dto = new ShiftAssignDto
                {
                    EmployeeId = employeeId,
                    ShiftId = shiftId,
                    WorkDate = workDate
                };

                var (success, message) = await _shiftApiService.AssignShiftAsync(dto);

                if (success)
                {
                    _dialogService.ShowMessage("Shift assigned successfully!");
                    await LoadScheduleAsync();
                }
                else
                {
                    _dialogService.ShowMessage(message);
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AssignShift Error: {ex.Message}");
                _dialogService.ShowMessage($"Error: {ex.Message}");
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Thêm vào ShiftScheduleViewModel.cs

        [RelayCommand]
        private async Task ApproveCompensation(EmployeeShiftResponseDto shift)
        {
            // Kiểm tra nếu ca là 'Absent' thì mới cho duyệt
            if (shift.Status != "Absent")
            {
                _dialogService.ShowMessage("Only 'Absent' shifts can be approved for compensation.");
                return;
            }

            var success = await _shiftApiService.ApproveCompensationAsync(shift.Id);
            if (success)
            {
                _dialogService.ShowMessage("Compensation approved successfully!");
                // Refresh lại dữ liệu sau khi duyệt
                await LoadScheduleAsync();
            }
            else
            {
                _dialogService.ShowMessage("Failed to approve compensation.");
            }
        }

        // Bổ sung logic cho việc mở Dialog chọn nhân viên
        [RelayCommand]
        private async Task OpenAssignShiftDialog(DateTime date)
        {
            var dialog = new BookStore_Management_AppDesktop.Views.Windows.DayDetailDialog(
                date,
                _shiftApiService,
                _employees
            );
            
            var result = dialog.ShowDialog();
            if (result == true)
            {
                await LoadScheduleAsync();
            }
        }

        [RelayCommand]
        private async Task ExportSchedule()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Exporting schedule...";

                int month = CurrentMonthStart.Month;
                int year = CurrentMonthStart.Year;

                await _exportService.ExportScheduleToExcelAsync(month, year, AllShifts);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ExportSchedule Error: {ex.Message}");
                _dialogService.ShowMessage($"Export failed: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    public class ScheduleCellViewModel : ObservableObject
    {
        public DateTime? Date { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public int DayNumber { get; set; }
        public bool IsDummy { get; set; } = false;
        public ObservableCollection<EmployeeShiftResponseDto> Shifts { get; set; } = new();
    }
}
