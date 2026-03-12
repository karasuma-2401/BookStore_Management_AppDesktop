using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.Views.UserControls;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        public Action CloseAction { get; set; }

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

        // variable for Remember ME
        [ObservableProperty]
        private bool _rememberMe = false;

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !IsLoading;
        }

        private void HandleRememberMe ()
        {
            if (RememberMe)
            {
                Settings.Default.RememberMe = true ;
                Settings.Default.SavedUsername = Username;
            }
            else
            {
                Settings.Default.RememberMe = false;
                Settings.Default.SavedUsername = String.Empty;
            }
            Settings.Default.Save();
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

<<<<<<< Updated upstream
=======
            Settings.Default.AccessToken = loginResult.AccessToken;
            Settings.Default.Save();

>>>>>>> Stashed changes
            IsSuccessMessage = true;
            ErrorMessage = "Login successful! Redirecting...";
            Debug.WriteLine(ErrorMessage);

            // RememberMe Logic
            HandleRememberMe();

            _navigationService.NavigateToMainWindow();
            CloseAction?.Invoke();
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
        #region ForgotPassword

        [ObservableProperty]
        private bool _isForgotPasswordFormVisible = false;

        [ObservableProperty]
        private int _forgotPasswordStep = 1;

        [ObservableProperty]
        private string _resetEmail = string.Empty;

        [ObservableProperty]
        private string _otpCode = string.Empty;

        [ObservableProperty]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        private string _confirmNewPassword = string.Empty;

        // Step 1: verify email have existed in DB

        [RelayCommand]
        private async Task VerifyEmail()
        {
            ErrorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(ResetEmail))
            {
                IsSuccessMessage = false;
                ErrorMessage = "Please enter your email address!";
                return;
            }

            IsLoading = true;
            await Task.Delay(2000);

            IsSuccessMessage = true;
            ErrorMessage = $"The OTP has been sent to your email {ResetEmail}";
            ForgotPasswordStep = 2;
            IsLoading = false;
        }

        // step 2 Send OTP to email, get OTP code
        [RelayCommand]
        private async Task VerifyOtp()
        {
            ErrorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(OtpCode) || OtpCode.Length != 6)
            {
                IsSuccessMessage = false;
                ErrorMessage = "OTP must be 6 digits";
                return;
            }

            IsLoading = true;
            await Task.Delay(20000);
            IsSuccessMessage = true;
            ErrorMessage = "Verification completed successfully";
            ForgotPasswordStep = 3;
            IsLoading = false;

        }

        [RelayCommand]
        private async Task ChangeToNewPassword()
        {
            ErrorMessage = string.Empty;
            if (NewPassword != ConfirmNewPassword)
            {
                IsSuccessMessage = false;
                ErrorMessage = "Passwords do not match";
                return;
            }

            IsLoading = true;
            await Task.Delay(2000);

            IsSuccessMessage = true;
            ErrorMessage = "Password changed successfully";
            SwitchToLogin();
            IsLoading = false;
        }


        #endregion

        [ObservableProperty]
        private ObservableCollection<string> _carouselBooks = new();
        public LoginViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            LoadMockBooks();

            RememberMe = Settings.Default.RememberMe;
            if (!RememberMe)
                Username = Settings.Default.SavedUsername;

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
            IsForgotPasswordFormVisible = false;
        }

        [RelayCommand]
        private void SwitchToLogin()
        {
            ErrorMessage = string.Empty;
            IsSignUpFormVisible = false;
            IsLoginFormVisible = true;
            IsForgotPasswordFormVisible = false;
        }
        [RelayCommand]
        private void SwitchToForgotPassword()
        {
            ErrorMessage = string.Empty;
            IsSignUpFormVisible = false;
            IsLoginFormVisible = false;

            ForgotPasswordStep = 1;
            IsForgotPasswordFormVisible = true;
        }
    }
}