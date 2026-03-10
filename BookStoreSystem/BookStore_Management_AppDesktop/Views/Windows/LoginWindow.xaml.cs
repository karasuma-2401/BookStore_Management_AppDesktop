using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BookStore_Management_AppDesktop.ViewModels;
using Wpf.Ui.Controls;
using System.Windows.Input;
namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class LoginWindow : FluentWindow
    {
        private readonly Random _random = new Random();
        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            viewModel.CloseAction = () => this.Close();
        }

        private void BookImagesBackground_Loaded(object sender, RoutedEventArgs e)
        {
            // check isActived object is Image and have Transform
            if (sender is System.Windows.Controls.Image img && img.RenderTransform is TranslateTransform transform)
            {
                // fly from 10px to 25px
                double randomDistance = _random.Next(-25, -10);
                // fly time from 2.5s to 4.5s
                double randomDuration = _random.NextDouble() *2 + 2;
                // delay from 0 to 2s
                double randomDelay = _random.NextDouble() * 2;
                var fluctuateAnimation = new DoubleAnimation
                {
                    To = randomDistance,
                    Duration = TimeSpan.FromSeconds(randomDuration),
                    BeginTime = TimeSpan.FromSeconds(randomDelay),
                    // parameter for restart
                    AutoReverse = true, 
                    // infinite loop
                    RepeatBehavior = RepeatBehavior.Forever,
                    EasingFunction = new SineEase {EasingMode = EasingMode.EaseInOut}
                };
                transform.BeginAnimation(TranslateTransform.YProperty, fluctuateAnimation);
            }
        }
    }
}
