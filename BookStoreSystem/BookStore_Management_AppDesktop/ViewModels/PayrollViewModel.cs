using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Services.API.EmployeeServices;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.Export;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.ShiftDTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public class PayrollRowViewModel : ObservableObject
    {
        public int EmployeeId { get; set; }
        public string? FullName { get; set; }
        public int TotalAssignedShifts { get; set; }
        public int WorkedShifts { get; set; }
        public int AbsentShifts { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal ActualSalary { get; set; }
        public PayslipDto? Payslip { get; set; }
    }

    public partial class PayrollViewModel : ObservableObject
    {
        private readonly IEmployeeShiftApiService _shiftApiService;
        private readonly IEmployeeApiService _employeeApiService;
        private readonly IDialogService _dialogService;
        private readonly IExportService _exportService;

        private List<Employee> _allEmployees = new();

        [ObservableProperty]
        private ObservableCollection<PayrollRowViewModel> payrollData = new();

        [ObservableProperty]
        private int selectedMonth;

        [ObservableProperty]
        private int selectedYear;

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private string loadingMessage = "Loading...";

        [ObservableProperty]
        private decimal totalPayroll = 0;

        [ObservableProperty]
        private int totalEmployees = 0;

        public List<int> AvailableYears { get; set; }
        public List<int> AvailableMonths { get; set; }

        public PayrollViewModel(IEmployeeShiftApiService shiftApiService,
                              IEmployeeApiService employeeApiService,
                              IDialogService dialogService,
                              IExportService exportService)
        {
            _shiftApiService = shiftApiService;
            _employeeApiService = employeeApiService;
            _dialogService = dialogService;
            _exportService = exportService;

            SelectedMonth = DateTime.Now.Month;
            SelectedYear = DateTime.Now.Year;

            AvailableMonths = Enumerable.Range(1, 12).ToList();
            AvailableYears = Enumerable.Range(DateTime.Now.Year - 5, 6).ToList();

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading employees...";

                var employees = await _employeeApiService.GetAllEmployeesAsync();
                _allEmployees = employees ?? new List<Employee>();

                TotalEmployees = _allEmployees.Count;

                await GeneratePayrollAsync();
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

        private bool _isGeneratingPayroll = false;

        [RelayCommand]
        private async Task GeneratePayroll()
        {
            await GeneratePayrollAsync();
        }

        private async Task GeneratePayrollAsync()
        {
            if (_isGeneratingPayroll) return;
            try
            {
                _isGeneratingPayroll = true;
                IsLoading = true;
                LoadingMessage = "Calculating payroll...";

                if (_allEmployees == null || !_allEmployees.Any())
                {
                    var employees = await _employeeApiService.GetAllEmployeesAsync();
                    _allEmployees = employees ?? new List<Employee>();
                    TotalEmployees = _allEmployees.Count;
                }

                PayrollData.Clear();
                TotalPayroll = 0;

                foreach (var employee in _allEmployees)
                {
                    var payslip = await _shiftApiService.GetPayrollAsync(employee.EmployeeId, SelectedMonth, SelectedYear);

                    if (payslip != null)
                    {
                        var rowViewModel = new PayrollRowViewModel
                        {
                            EmployeeId = payslip.EmployeeId,
                            FullName = payslip.FullName,
                            TotalAssignedShifts = payslip.TotalAssignedShifts,
                            WorkedShifts = payslip.WorkedShifts,
                            AbsentShifts = payslip.AbsentShifts,
                            BaseSalary = payslip.Salary,
                            ActualSalary = payslip.ActualSalary,
                            Payslip = payslip
                        };

                        PayrollData.Add(rowViewModel);
                        TotalPayroll += payslip.ActualSalary;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GeneratePayroll Error: {ex.Message}");
                _dialogService.ShowMessage($"Failed to generate payroll: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                _isGeneratingPayroll = false;
            }
        }

        [RelayCommand]
        private async Task ExportPayroll()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Exporting payroll...";

                var payslips = PayrollData
                    .Select(row => row.Payslip)
                    .OfType<PayslipDto>()
                    .ToList();

                if (payslips.Count == 0)
                {
                    _dialogService.ShowMessage("No payroll data to export.");
                    return;
                }

                await _exportService.ExportPayrollToExcelAsync(SelectedMonth, SelectedYear, payslips);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ExportPayroll Error: {ex.Message}");
                _dialogService.ShowMessage($"Failed to export payroll: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSelectedMonthChanged(int value)
        {
            _ = GeneratePayrollAsync();
        }

        partial void OnSelectedYearChanged(int value)
        {
            _ = GeneratePayrollAsync();
        }
    }
}
