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
using System.Reflection;
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
        [ObservableProperty] private Axis[] _yAxes = Array.Empty<Axis>();

        [ObservableProperty] private ISeries[] _inventorySeries = Array.Empty<ISeries>();
        [ObservableProperty] private Axis[] _inventoryXAxes = Array.Empty<Axis>();
        [ObservableProperty] private Axis[] _inventoryYAxes = Array.Empty<Axis>();

        [ObservableProperty] private ObservableCollection<TopBookDto> _topBooks = new ObservableCollection<TopBookDto>();
        [ObservableProperty] private ObservableCollection<InventoryReportResponseDTO> _inventoryReports = new ObservableCollection<InventoryReportResponseDTO>();
        [ObservableProperty] private ObservableCollection<DebtReportResponseDTO> _debtReports = new ObservableCollection<DebtReportResponseDTO>();

        public ReportViewModel(IReportApiService reportApi, IDialogService dialogService, IExportService exportService)
        {
            _reportApi = reportApi;
            _dialogService = dialogService;
            _exportService = exportService;
            XAxes = new Axis[] { new Axis { Name = "Days in Month" } };
            YAxes = new Axis[] { new Axis() };
            InventoryXAxes = new Axis[] { new Axis { Name = "Books" } };
            InventoryYAxes = new Axis[] { new Axis { Name = "Quantity" } };
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
                            Stroke = new SolidColorPaint(SKColor.Parse("#0078D4")) { StrokeThickness = 4 },
                            Fill = new LinearGradientPaint(
                                new[] { SKColor.Parse("#330078D4"), SKColor.Parse("#000078D4") },
                                new SKPoint(0.5f, 0),
                                new SKPoint(0.5f, 1)
                            ),
                            GeometrySize = 10,
                            GeometryStroke = new SolidColorPaint(SKColor.Parse("#0078D4")) { StrokeThickness = 2 },
                            GeometryFill = new SolidColorPaint(SKColors.White),
                            LineSmoothness = 0.5
                        }
                    };

                    XAxes = new Axis[]
                    {
                        new Axis
                        {
                            Labels = dayLabels,
                            Name = "Days in Month",
                            LabelsPaint = new SolidColorPaint(SKColor.Parse("#64748B")),
                            NamePaint = new SolidColorPaint(SKColor.Parse("#0F172A")),
                            TextSize = 13
                        }
                    };

                    YAxes = new Axis[]
                    {
                        new Axis
                        {
                            LabelsPaint = new SolidColorPaint(SKColor.Parse("#64748B")),
                            TextSize = 13,
                            Labeler = value => (value / 1000).ToString("N0") + "k",
                            MinStep = 1000
                        }
                    };
                }
                else
                {
                    _dialogService.ShowMessage("Could not retrieve monthly report data from the server.");
                }

                await _reportApi.GenerateInventoryReportAsync(SelectedMonth, SelectedYear);

                var inventoryData = await _reportApi.GetInventoryReportsAsync(SelectedMonth, SelectedYear);
                InventoryReports.Clear();
                if (inventoryData != null)
                {
                    var bookNames = new List<string>();
                    var finalStocks = new List<int>();

                    foreach (var item in inventoryData)
                    {
                        InventoryReports.Add(item);

                        // Lấy BookId từ DTO và thêm chữ "Book #" cho biểu đồ đẹp hơn
                        string bName = string.IsNullOrWhiteSpace(item.BookName) ? $"Book #{item.BookId}" : item.BookName;
                        int bStock = item.ClosingStock;

                        bookNames.Add(bName);
                        finalStocks.Add(bStock);
                    }

                    InventorySeries = new ISeries[]
                    {
                        new ColumnSeries<int>
                        {
                            Values = finalStocks.ToArray(),
                            Name = "Final Stock",
                            // Style màu Gradient hiện đại (Xanh ngọc sang Xanh lục)
                            Fill = new LinearGradientPaint(
                                new[] { SKColor.Parse("#34D399"), SKColor.Parse("#059669") },
                                new SKPoint(0.5f, 0),
                                new SKPoint(0.5f, 1)
                            ),
                            MaxBarWidth = 45, // Cột mập mạp hơn
                            Rx = 8, // Bo góc tròn trịa
                            Ry = 8
                        }
                    };

                    InventoryXAxes = new Axis[]
                    {
                        new Axis
                        {
                            Labels = bookNames.ToArray(),
                            LabelsPaint = new SolidColorPaint(SKColor.Parse("#64748B")),
                            TextSize = 13,
                            LabelsRotation = 15 // Nghiêng nhẹ chữ để không bị đè lên nhau
                        }
                    };

                    InventoryYAxes = new Axis[]
                    {
                        new Axis
                        {
                            Name = "Stock Quantity",
                            LabelsPaint = new SolidColorPaint(SKColor.Parse("#64748B")),
                            NamePaint = new SolidColorPaint(SKColor.Parse("#0F172A")),
                            TextSize = 13
                        }
                    };
                }

                await _reportApi.GenerateDebtReportAsync(SelectedMonth, SelectedYear);

                var debtData = await _reportApi.GetDebtReportsAsync(SelectedMonth, SelectedYear);
                DebtReports.Clear();
                if (debtData != null)
                {
                    foreach (var item in debtData)
                    {
                        DebtReports.Add(item);
                    }
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