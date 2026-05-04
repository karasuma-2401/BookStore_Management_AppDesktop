using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Models.DTOs;
using BookStore_Management_AppDesktop.Services.API.Import;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows; // ĐÃ FIX LỖI Application và MessageBox

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

        // ĐÃ FIX CẢNH BÁO CS8618: Thêm dấu ? để cho phép Null
        public object? OriginalData { get; set; }
    }

    public partial class ImportHistoryViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IImportApiService _importApiService;

        [ObservableProperty]
        private ObservableCollection<ImportHistoryUIModel> _importHistory = new ObservableCollection<ImportHistoryUIModel>();

        // ĐÃ FIX LỖI CS0103: Tự khai báo thuộc tính IsLoading
        [ObservableProperty]
        private bool _isLoading;

        public ImportHistoryViewModel(INavigationService navigationService, IImportApiService importApiService)
        {
            _navigationService = navigationService;
            _importApiService = importApiService;
        }

        public override async Task LoadDataAsync()
        {
            IsLoading = true;
            var dataFromApi = await _importApiService.GetAllImportsAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                ImportHistory.Clear();
                foreach (var item in dataFromApi.OrderByDescending(x => x.ImportDate))
                {
                    ImportHistory.Add(new ImportHistoryUIModel
                    {
                        ImportId = item.ImportId,
                        ImportDate = item.ImportDate.ToLocalTime(), // Đổi giờ UTC thành giờ Việt Nam
                        UserId = item.UserId,
                        EmployeeName = string.IsNullOrWhiteSpace(item.UserName) ? "Unknown" : item.UserName,
                        TotalItems = item.Details.Sum(d => d.Quantity),
                        TotalAmount = item.Details.Sum(d => d.Quantity * d.ImportPrice),
                        OriginalData = item
                    });
                }
            });
            IsLoading = false;
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