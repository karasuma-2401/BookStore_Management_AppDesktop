using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Models.DTOs;
using BookStore_Management_AppDesktop.Services.API.Import;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.Services.Realtime; 
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public class ImportHistoryUIModel
    {
        public int ImportId { get; set; }
        public DateTime ImportDate { get; set; }
        public int UserId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        public object? OriginalData { get; set; }
    }

    public partial class ImportHistoryViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IImportApiService _importApiService;
        private readonly IBookHubService _hubService;
        private ObservableCollection<ImportHistoryUIModel> _allImportHistory = new ObservableCollection<ImportHistoryUIModel>();

        [ObservableProperty] private ObservableCollection<ImportHistoryUIModel> _importHistory = new ObservableCollection<ImportHistoryUIModel>();
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private DateTime? _filterStartDate = DateTime.Now.AddMonths(-1);
        [ObservableProperty] private DateTime? _filterEndDate = DateTime.Now;

        public ImportHistoryViewModel(
            INavigationService navigationService,
            IImportApiService importApiService,
            IBookHubService hubService) 
        {
            _navigationService = navigationService;
            _importApiService = importApiService;
            _hubService = hubService;
            _hubService.ImportCreated += OnImportRealtimeCreated;
        }

        private void OnImportRealtimeCreated()
        {
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await LoadDataAsync();
            });
        }

        public override async Task LoadDataAsync()
        {
            IsLoading = true;
            var dataFromApi = await _importApiService.GetAllImportsAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                _allImportHistory.Clear();
                foreach (var item in dataFromApi.OrderByDescending(x => x.ImportDate))
                {
                    _allImportHistory.Add(new ImportHistoryUIModel
                    {
                        ImportId = item.ImportId,
                        ImportDate = item.ImportDate.ToLocalTime(), 
                        UserId = item.UserId,
                        EmployeeName = string.IsNullOrWhiteSpace(item.UserName) ? "Unknown" : item.UserName,
                        TotalItems = item.Details.Sum(d => d.Quantity),
                        TotalAmount = item.Details.Sum(d => d.Quantity * d.ImportPrice),
                        OriginalData = item
                    });
                }
                ApplyCurrentFilter();
            });
            IsLoading = false;
        }

        private void ApplyCurrentFilter()
        {
            ImportHistory.Clear();
            var filtered = _allImportHistory.AsEnumerable();

            if (FilterStartDate.HasValue)
            {
                var startDate = FilterStartDate.Value.Date;
                filtered = filtered.Where(x => x.ImportDate.Date >= startDate);
            }

            if (FilterEndDate.HasValue)
            {
                var endDate = FilterEndDate.Value.Date.AddDays(1);
                filtered = filtered.Where(x => x.ImportDate.Date < endDate);
            }

            foreach (var item in filtered)
            {
                ImportHistory.Add(item);
            }
        }

        partial void OnFilterStartDateChanged(DateTime? value)
        {
            if (value.HasValue && FilterEndDate.HasValue && value > FilterEndDate)
            {
                FilterStartDate = FilterEndDate;
            }
        }

        partial void OnFilterEndDateChanged(DateTime? value)
        {
            if (value.HasValue && FilterStartDate.HasValue && value < FilterStartDate)
            {
                FilterEndDate = FilterStartDate;
            }
        }

        [RelayCommand]
        private void ApplyDateFilter()
        {
            if (FilterStartDate.HasValue && FilterEndDate.HasValue && FilterStartDate > FilterEndDate)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Start date cannot be greater than end date!", "Invalid Date Range", MessageBoxButton.OK, MessageBoxImage.Warning);
                });
                return;
            }

            ApplyCurrentFilter();
        }

        [RelayCommand]
        private void NavigateToCreateImport()
        {
            _navigationService.NavigateTo(PageType.CreateImport);
        }

        [RelayCommand]
        private void ViewDetail(ImportHistoryUIModel selectedImport)
        {
            if (selectedImport == null) return;

            var detailWindow = new BookStore_Management_AppDesktop.Views.Windows.ImportDetailWindow(selectedImport);

            detailWindow.ShowDialog();
        }
    }
}