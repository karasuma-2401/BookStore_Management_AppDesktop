using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;
using BookStore_Management_AppDesktop.Services.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class ImportHistoryViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        public ImportHistoryViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        // Lệnh này nối với nút "Nhập hàng mới" ở UI
        [RelayCommand]
        private void NavigateToCreateImport()
        {
            _navigationService.NavigateTo(PageType.CreateImport);
        }

        // Lệnh này nối với nút "Chi tiết" ở mỗi dòng của DataGrid
        [RelayCommand]
        private void ViewDetail(object selectedImport)
        {
            // Tạm để trống, sẽ làm ở task sau
        }
    }
}
