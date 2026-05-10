using BookStore_Management_AppDesktop.Models.DTOs.ReportDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.ReportServices;
using BookStore_Management_AppDesktop.Services.Export;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class ReportViewModel : BaseViewModel
    {
        private readonly IReportApiService _reportApi;
        private readonly IDialogService _dialogService;
        private readonly IExportService _exportService;

        [ObservableProperty] private int _selectedMonth = DateTime.Now.Month;
        [ObservableProperty] private int _selectedYear = DateTime.Now.Year;

        public List<int> Months { get; } = Enumerable.Range(1, 12).ToList();
        public List<int> Years { get; } = Enumerable.Range(2023, 10).ToList();

        [ObservableProperty] private decimal _totalRevenue;
        [ObservableProperty] private decimal _totalImportCost;
        [ObservableProperty] private decimal _profit;
        [ObservableProperty] private int _totalBooksSold;
        [ObservableProperty] private bool _isLoading = false;

        [ObservableProperty] private ISeries[] _revenueSeries = Array.Empty<ISeries>();
        [ObservableProperty] private Axis[] _xAxes = Array.Empty<Axis>();
        [ObservableProperty] private ObservableCollection<TopBookDto> _topBooks = new ObservableCollection<TopBookDto>();

        public ReportViewModel(IReportApiService reportApi, IDialogService dialogService, IExportService exportService)
        {
            _reportApi = reportApi;
            _dialogService = dialogService;
            _exportService = exportService;
            XAxes = new Axis[] { new Axis { Name = "Days of Month" } };
        }

        public override async Task LoadDataAsync()
        {
            await GenerateReportAsync();
        }

        [RelayCommand]
        private async Task GenerateReportAsync()
        {
            IsLoading = true;
            try
            {
                var data = await _reportApi.GetMonthlyReportAsync(SelectedMonth, SelectedYear);

                if (data != null)
                {
                    TotalRevenue = data.TotalRevenue;
                    TotalImportCost = data.TotalImportCost;
                    Profit = data.Profit;
                    TotalBooksSold = data.TotalBooksSold;
                    TopBooks.Clear();
                    foreach (var book in data.TopSellingBooks)
                    {
                        TopBooks.Add(book);
                    }
                    var revenueValues = data.DailyData.Select(x => x.Revenue).ToArray();
                    var dayLabels = data.DailyData.Select(x => x.Day.ToString()).ToArray();

                    RevenueSeries = new ISeries[]
                    {
                        new LineSeries<decimal>
                        {
                            Values = revenueValues,
                            Name = "Revenue",
                            Stroke = new SolidColorPaint(SKColors.SpringGreen) { StrokeThickness = 3 },
                            Fill = null, 
                            GeometrySize = 8,
                            GeometryStroke = new SolidColorPaint(SKColors.SpringGreen) { StrokeThickness = 2 }
                        }
                    };

                    XAxes = new Axis[]
                    {
                        new Axis
                        {
                            Labels = dayLabels,
                            Name = "Days in Month",
                            LabelsPaint = new SolidColorPaint(SKColors.LightGray)
                        }
                    };
                }
                else
                {
                    _dialogService.ShowMessage("Could not retrieve report data from the server.");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Error generating report: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ExportToExcelAsync()
        {
            await _exportService.ExportReportToExcelAsync(
                SelectedMonth,
                SelectedYear,
                TotalRevenue,
                TotalImportCost,
                Profit,
                TotalBooksSold,
                TopBooks);
        }
    }
}