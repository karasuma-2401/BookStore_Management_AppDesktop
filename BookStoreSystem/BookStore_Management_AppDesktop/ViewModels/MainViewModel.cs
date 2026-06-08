using BookStore_Management_AppDesktop.Helpers;
using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Services.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        public bool IsAdmin => AppSession.IsAdmin;
        public bool IsNotEmployee => AppSession.IsAdmin;  // Only show for Admin, not for Employee


        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

        }

        [RelayCommand]
        private void Navigate(string pageName)
        {
            if (Enum.TryParse(pageName, out PageType pageType))
            {
                _navigationService.NavigateTo(pageType);
            }
        }

        [RelayCommand]
        private void Logout()
        {
            // Clear settings or session
            BookStore_Management_AppDesktop.Settings.Default.AccessToken = string.Empty;
            BookStore_Management_AppDesktop.Settings.Default.Save();
            
            AppSession.CurrentRole = "staff";

            // Navigate to Login
            _navigationService.NavigateToLoginWindow();

            // Close main window
            foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
            {
                if (window is BookStore_Management_AppDesktop.Views.Windows.MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
