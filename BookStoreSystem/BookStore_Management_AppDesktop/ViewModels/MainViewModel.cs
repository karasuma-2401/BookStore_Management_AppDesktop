using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Services.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

 
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
    }
}
