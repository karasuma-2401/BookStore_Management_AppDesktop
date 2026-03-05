using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _username = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _password = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _carouselBooks = new();
        public LoginViewModel()
        {
            LoadMockBooks();
        }
        private void LoadMockBooks()
        {
            CarouselBooks.Add("");
            CarouselBooks.Add("");
            CarouselBooks.Add("");
            CarouselBooks.Add("");
            CarouselBooks.Add("");
            CarouselBooks.Add("");
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        [RelayCommand(CanExecute = nameof(CanLogin))]
        private void Login ()
        {
            System.Diagnostics.Debug.WriteLine($"Thực hiện đăng nhập cho: {Username}");
        }
        [RelayCommand]
        private void NavigateToSignUp()
        {
            System.Diagnostics.Debug.WriteLine("Chuyển hướng sang Sign Up");
        }
    }
}
