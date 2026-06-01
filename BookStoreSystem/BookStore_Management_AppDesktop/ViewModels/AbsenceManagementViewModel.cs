    using BookStore_Management_AppDesktop.Services.API;
    using BookStore_Management_AppDesktop.Services;
    using CommunityToolkit.Mvvm.ComponentModel;
using BookStore_Management_AppDesktop.Models.DTOs.ShiftDTOs;
using CommunityToolkit.Mvvm.Input;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    namespace BookStore_Management_AppDesktop.ViewModels
    {
        public partial class AbsenceManagementViewModel : ObservableObject
        {
            private readonly IEmployeeShiftApiService _shiftApiService;
            private readonly IDialogService _dialogService;

            [ObservableProperty]
            private ObservableCollection<EmployeeShiftResponseDto> absentShifts = new();

            [ObservableProperty]
            private ObservableCollection<EmployeeShiftResponseDto> selectedAbsentShifts = new();

            [ObservableProperty]
            private DateTime currentMonthStart;

            [ObservableProperty]
            private DateTime filterStartDate;

            [ObservableProperty]
            private DateTime filterEndDate;

            [ObservableProperty]
            private bool isLoading = false;

            [ObservableProperty]
            private string loadingMessage = "Loading...";

            [ObservableProperty]
            private int totalAbsentShifts = 0;

            private List<EmployeeShiftResponseDto> _allAbsentShifts = new();

            public AbsenceManagementViewModel(IEmployeeShiftApiService shiftApiService,
                                            IDialogService dialogService)
            {
                _shiftApiService = shiftApiService;
                _dialogService = dialogService;

                CurrentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                FilterStartDate = CurrentMonthStart;
                FilterEndDate = CurrentMonthStart.AddMonths(1).AddDays(-1);
                _ = InitializeAsync();
            }

            private async Task InitializeAsync()
            {
                await LoadAbsentShiftsAsync();
            }

            private async Task LoadAbsentShiftsAsync()
            {
                try
                {
                    IsLoading = true;
                    LoadingMessage = "Loading absent shifts...";

                    var shifts = await _shiftApiService.GetAbsentShiftsAsync(FilterStartDate, FilterEndDate);

                    _allAbsentShifts = shifts.ToList();
                    ApplyFilters();

                    TotalAbsentShifts = AbsentShifts.Count;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"LoadAbsentShifts Error: {ex.Message}");
                    _dialogService.ShowMessage($"Failed to load absent shifts: {ex.Message}");
                }
                finally
                {
                    IsLoading = false;
                }
            }

            private void ApplyFilters()
            {
                AbsentShifts.Clear();
                foreach (var shift in _allAbsentShifts.Where(s => s.Status.Equals("Absent", StringComparison.OrdinalIgnoreCase)))
                {
                    shift.IsSelected = false; // Reset selection state when loading/filtering
                    AbsentShifts.Add(shift);
                }
            }

            [RelayCommand]
            private void ToggleSelection(EmployeeShiftResponseDto shift)
            {
                if (shift == null) return;
                shift.IsSelected = !shift.IsSelected;
            }

            [RelayCommand]
            private void SelectAll()
            {
                foreach (var shift in AbsentShifts)
                {
                    shift.IsSelected = true;
                }
            }

            [RelayCommand]
            private void ClearSelection()
            {
                foreach (var shift in AbsentShifts)
                {
                    shift.IsSelected = false;
                }
            }

            [RelayCommand]
            private async Task ApproveBulkCompensation()
            {
                var selectedShifts = AbsentShifts.Where(s => s.IsSelected).ToList();
                if (selectedShifts.Count == 0)
                {
                    _dialogService.ShowMessage("Please select at least one absent shift.");
                    return;
                }

                var confirmed = _dialogService.ShowConfirmation(
                    $"Are you sure you want to approve compensation for {selectedShifts.Count} absent shift(s)?",
                    "Approve",
                    false);

                if (!confirmed) return;

                try
                {
                    IsLoading = true;
                    var assignmentIds = selectedShifts.Select(s => s.Id).ToList();
                    var success = await _shiftApiService.ApproveBulkCompensationAsync(assignmentIds);

                    if (success)
                    {
                        _dialogService.ShowMessage($"Compensation approved for {selectedShifts.Count} shifts! Employees will be paid.");
                        await LoadAbsentShiftsAsync();
                    }
                    else
                    {
                        await _dialogService.ShowErrorAsync("Error", "Failed to approve compensations.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ApproveBulkCompensation Error: {ex.Message}");
                    await _dialogService.ShowErrorAsync("Error", $"Error: {ex.Message}");
                }
                finally
                {
                    IsLoading = false;
                }
            }

            [RelayCommand]
            private async Task PreviousMonth()
            {
                CurrentMonthStart = CurrentMonthStart.AddMonths(-1);
                FilterStartDate = CurrentMonthStart;
                FilterEndDate = CurrentMonthStart.AddMonths(1).AddDays(-1);
                await LoadAbsentShiftsAsync();
            }

            [RelayCommand]
            private async Task NextMonth()
            {
                CurrentMonthStart = CurrentMonthStart.AddMonths(1);
                FilterStartDate = CurrentMonthStart;
                FilterEndDate = CurrentMonthStart.AddMonths(1).AddDays(-1);
                await LoadAbsentShiftsAsync();
            }

            [RelayCommand]
            private async Task ApplyDateFilter()
            {
                await LoadAbsentShiftsAsync();
            }

            [RelayCommand]
            private async Task ApproveCompensation(EmployeeShiftResponseDto shift)
            {
                if (shift == null || !shift.Status.Equals("Absent", StringComparison.OrdinalIgnoreCase)) return;

                var confirmed = await _dialogService.ShowConfirmationAsync(
                    "Approve Compensation",
                    $"Approve compensation for {shift.FullName}'s absent shift on {shift.WorkDate}?");

                if (!confirmed) return;

                try
                {
                    IsLoading = true;
                    var success = await _shiftApiService.ApproveCompensationAsync(shift.Id);

                    if (success)
                    {
                        await _dialogService.ShowSuccessAsync("Success", "Compensation approved! Employee will be paid.");
                        await LoadAbsentShiftsAsync();
                    }
                    else
                    {
                        await _dialogService.ShowErrorAsync("Error", "Failed to approve compensation.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ApproveCompensation Error: {ex.Message}");
                    await _dialogService.ShowErrorAsync("Error", $"Error: {ex.Message}");
                }
                finally
                {
                    IsLoading = false;
                }
            }

            [RelayCommand]
            private async Task RefreshData()
            {
                SelectedAbsentShifts.Clear();
                await LoadAbsentShiftsAsync();
            }
        }
    }
