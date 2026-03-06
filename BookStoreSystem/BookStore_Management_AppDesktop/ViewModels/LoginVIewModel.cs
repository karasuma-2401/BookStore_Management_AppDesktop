using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isSuccessMessage = false;

        #region Login

        [ObservableProperty]
        private bool _isLoginFormVisible = true;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _username = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _password = string.Empty;

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !IsLoading;
        }

        [RelayCommand(CanExecute = nameof(CanLogin))]
        private async Task Login()
        {
            ErrorMessage = String.Empty;
            IsLoading = true;

            // await time to connect with BE
            await Task.Delay(1000);

            // admin username and password
            if (Username != "admin" ||  Password != "123")
            {
                IsSuccessMessage = false;
                ErrorMessage = "The username or password is incorrect!";
                IsLoading = false;
                return;
            }

            IsSuccessMessage = true;
            ErrorMessage = "Login successful! Redirecting...";
            Debug.WriteLine(ErrorMessage);



            // call to feature
            IsLoading = false;
        }
        #endregion
        #region Sign up

        [ObservableProperty]
        private bool _isSignUpFormVisible = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private string _signUpFullName = string.Empty;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private string _signUpUsername = string.Empty;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private string _signUpPassword = string.Empty;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private string _signUpConfirmPassword = string.Empty;

        private bool CanSignUp()
        {
            return !String.IsNullOrWhiteSpace(SignUpFullName) && 
                    !String.IsNullOrWhiteSpace(SignUpUsername) &&
                    !String.IsNullOrWhiteSpace(SignUpPassword) &&
                    !String.IsNullOrWhiteSpace(SignUpConfirmPassword) &&
                    SignUpPassword == SignUpConfirmPassword &&
                    !IsLoading;
        }
        
        [RelayCommand(CanExecute =nameof(CanSignUp))]
        private async Task SignUp()
        {
            ErrorMessage = string.Empty;
            IsLoading = true;

            // await time 1s to connection with BE
            await Task.Delay (1000);
            if (SignUpUsername == "admin")
            {
                IsSuccessMessage = false;
                ErrorMessage = "This username already exists.";
                IsLoading = false;
                return;
            }
            IsSuccessMessage = true;
            ErrorMessage = "Registration successful! Redirecting to the login page...";
            Debug.WriteLine(ErrorMessage);

            // Return to Login Window
            await Task.Delay(2000);
            SwitchToLogin();

            IsLoading = false;
        }
        #endregion

        [ObservableProperty]
        private ObservableCollection<string> _carouselBooks = new();
        public LoginViewModel()
        {
            LoadMockBooks();
        }
        // load background with full book from resource -> images
        private void LoadMockBooks()
        {
            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images");
            if (Directory.Exists(imagesFolder))
            {
                var imageFiles = Directory
                            .GetFiles(imagesFolder, "*.webp")
                            .Select(Path.GetFileName);

                foreach (var file in imageFiles)
                {
                    String fullPath = Path.Combine(imagesFolder, file);
                    CarouselBooks.Add(fullPath);
                }
            }
        }

        [RelayCommand]
        private void SwitchToSignUp()
        {
            ErrorMessage = string.Empty;
            IsLoginFormVisible = false;
            IsSignUpFormVisible = true;
        }

        [RelayCommand]
        private void SwitchToLogin()
        {
            ErrorMessage = string.Empty;
            IsSignUpFormVisible = false;
            IsLoginFormVisible = true;
        }
    }
}
