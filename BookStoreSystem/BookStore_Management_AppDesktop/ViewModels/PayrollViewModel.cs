using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Services.API.EmployeeServices;
using BookStore_Management_AppDesktop.Services;
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
                              IDialogService dialogService)
        {
            _shiftApiService = shiftApiService;
            _employeeApiService = employeeApiService;
            _dialogService = dialogService;

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

        [RelayCommand]
        private async Task GeneratePayroll()
        {
            await GeneratePayrollAsync();
        }

        private async Task GeneratePayrollAsync()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Calculating payroll...";

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
            }
        }

        [RelayCommand]
        private async Task ExportPayroll()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Exporting payroll...";

                var filename = $"Payroll_{SelectedMonth:D2}_{SelectedYear}.csv";
                var content = new System.Text.StringBuilder();

                // Header
                content.AppendLine("Employee ID,Full Name,Total Assigned Shifts,Worked Shifts,Absent Shifts,Base Salary,Actual Salary");

                // Data rows
                foreach (var row in PayrollData)
                {
                    content.AppendLine($"{row.EmployeeId},{row.FullName},{row.TotalAssignedShifts},{row.WorkedShifts},{row.AbsentShifts},{row.BaseSalary},{row.ActualSalary}");
                }

                // Footer
                content.AppendLine($"\n,TOTAL PAYROLL,,,,,{TotalPayroll}");

                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var filepath = Path.Combine(desktopPath, filename);

                await File.WriteAllTextAsync(filepath, content.ToString());

                _dialogService.ShowMessage($"Payroll exported to {filepath}");
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
