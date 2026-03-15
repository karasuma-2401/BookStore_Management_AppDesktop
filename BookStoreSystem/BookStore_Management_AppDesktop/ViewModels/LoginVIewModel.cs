using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.Views.UserControls;
using System.ComponentModel.DataAnnotations;
using BookStore_Management_AppDesktop.Services;
using System.Windows.Threading;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class LoginViewModel : ObservableValidator
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;
        public Action CloseAction { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isSuccessMessage = false;



        #region Login

        [ObservableProperty]
        private bool _isLoginFormVisible = true;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Username cannot be empty")]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _username = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "password cannot be empty")]
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

            // call API to connect to DB
            var loginResult = await _authService.LoginAsync(Username, Password);

            // admin username and password
            if (loginResult == null)
            {
                IsSuccessMessage = false;
                ErrorMessage = "Incorrect username or password";
                IsLoading = false;
                return;
            }

            Settings.Default.AccessToken = loginResult.AccessToken;
            Settings.Default.Save();

            IsSuccessMessage = true;
            ErrorMessage = "Login successful! Redirecting...";

            // RememberMe Logic
            HandleRememberMe();

            _navigationService.NavigateToMainWindow();
            CloseAction?.Invoke();
            IsLoading = false;
        }
        #endregion
        #region ForgotPassword

        [ObservableProperty]
        private bool _isForgotPasswordFormVisible = false;

        [ObservableProperty]
        private int _forgotPasswordStep = 1;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Invalid email format (e.g., example@gmail.com")]
        [NotifyCanExecuteChangedFor(nameof(VerifyEmailCommand))]
        private string _resetEmail = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
        private string _otpCode1 = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
        private string _otpCode2 = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
        private string _otpCode3 = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
        private string _otpCode4 = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
        private string _otpCode5 = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
        private string _otpCode6 = string.Empty;

        [ObservableProperty]
        private string _otpCode = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "New Password is required")]
        [NotifyCanExecuteChangedFor(nameof(ChangeToNewPasswordCommand))]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Confirm new password is required")]
        [NotifyCanExecuteChangedFor(nameof(ChangeToNewPasswordCommand))]
        private string _confirmPassword = string.Empty;

        public void IntegrateOtpCode ()
        {
            OtpCode = OtpCode1 + OtpCode2 + OtpCode3 + OtpCode4 + OtpCode5 + OtpCode6;
        }

        #region 
        private DispatcherTimer _countdownTimer;
        private int _timeRemaining;

        [ObservableProperty]
        private string _countdownText = "05:00";

        [ObservableProperty]
        private bool _isResendVisible = false;

        private void UpdateCountdownText ()
        {
            TimeSpan time = TimeSpan.FromSeconds(_timeRemaining);
            CountdownText = time.ToString(@"mm\:ss");
        }
        private void StartCountdown()
        {
            _timeRemaining = 300;
            IsResendVisible = false;
            UpdateCountdownText();

            if (_countdownTimer == null)
            {
                _countdownTimer = new DispatcherTimer();
                _countdownTimer.Interval = TimeSpan.FromSeconds(1);
                _countdownTimer.Tick += Timer_Tick;
            }

            _countdownTimer.Start();
        }

        private void Timer_Tick (object sender, EventArgs e)
        {
            if (_timeRemaining > 0)
            {
                _timeRemaining--;
                UpdateCountdownText();
            }
            else
            {
                _countdownTimer.Stop();
                CountdownText = "00:00";
                IsResendVisible = true;

                IsSuccessMessage = false;
                ErrorMessage = "OTP Code is expired. Please resend!";
            }
        }

        #endregion

        // Step 1: verify email have existed in DB
        private bool CanVerifyEmail()
        {
            return !string.IsNullOrWhiteSpace(_resetEmail);
        }
        [RelayCommand(CanExecute = nameof(CanVerifyEmail))]
        private async Task VerifyEmail()
        {
            ErrorMessage = string.Empty;

            IsLoading = true;
            var success = await _authService.ForgotPasswordAsync(ResetEmail);
            if (!success)
            {
                IsSuccessMessage = false;
                ErrorMessage = "Email is not existed";
                IsLoading = false;
                return;
            }

            IsSuccessMessage = true;
            ErrorMessage = $"The OTP has been sent to your email";
            ForgotPasswordStep = 2;

            StartCountdown();

            IsLoading = false;
        }

        // step 2 Send OTP to email, get OTP code
        private bool CanVerifyOtp()
        {
            return !string.IsNullOrWhiteSpace(OtpCode1) &&
                   !string.IsNullOrWhiteSpace(OtpCode2) &&
                   !string.IsNullOrWhiteSpace(OtpCode3) &&
                   !string.IsNullOrWhiteSpace(OtpCode4) &&
                   !string.IsNullOrWhiteSpace(OtpCode5) &&
                   !string.IsNullOrWhiteSpace(OtpCode6);
        }
        [RelayCommand(CanExecute = nameof(CanVerifyOtp))]
        private async Task VerifyOtp()
        {
            ErrorMessage = string.Empty;
            IntegrateOtpCode();
            if (string.IsNullOrWhiteSpace(OtpCode) || OtpCode.Length != 6)
            {
                IsSuccessMessage = false;
                ErrorMessage = "OTP must be 6 digits";
                return;
            }

            IsLoading = true;
            IsSuccessMessage = true;
            ErrorMessage = "Verification completed successfully";
            _countdownTimer.Stop();
            ForgotPasswordStep = 3;
            IsLoading = false;

        }
        private bool CanChangeToNewPassword()
        {
            return !string.IsNullOrWhiteSpace(NewPassword) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword);
        }
        [RelayCommand(CanExecute = nameof(CanChangeToNewPassword))]
        private async Task ChangeToNewPassword()
        {
            ErrorMessage = string.Empty;
            if (NewPassword != ConfirmPassword)
            {
                IsSuccessMessage = false;
                ErrorMessage = "Passwords do not match";
                return;
            }

            IsLoading = true;
            var success = await _authService.ResetPasswordAsync(OtpCode, NewPassword, ConfirmPassword);
            if (!success)
            {
                IsSuccessMessage = false;
                ErrorMessage = "OTP Code is expired";
                IsLoading = false;
                return;
            }

            IsSuccessMessage = true;
            ErrorMessage = "Password changed successfully";
            SwitchToLogin();
            IsLoading = false;
        }


        #endregion

        [ObservableProperty]
        private ObservableCollection<string> _carouselBooks = new();
        public LoginViewModel(INavigationService navigationService, IAuthService authService)
        {
            _navigationService = navigationService;
            _authService = authService;
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
        private void SwitchToLogin()
        {
            ClearErrors();
            ErrorMessage = string.Empty;
            IsLoginFormVisible = true;
            IsForgotPasswordFormVisible = false;
            _countdownTimer?.Stop();
        }
        [RelayCommand]
        private void SwitchToForgotPassword()
        {
            ClearErrors();
            ErrorMessage = string.Empty;
            IsLoginFormVisible = false;

            _countdownTimer?.Stop();

            ResetEmail = string.Empty;
            OtpCode1 = OtpCode2 = OtpCode3 = OtpCode4 = OtpCode5 = OtpCode6 = string.Empty;
            NewPassword = ConfirmPassword = string.Empty;

            ForgotPasswordStep = 1;
            IsForgotPasswordFormVisible = true;
        }
    }
}