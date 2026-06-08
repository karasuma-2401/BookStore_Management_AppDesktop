using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BookStore_Management_AppDesktop.ViewModels;
using Wpf.Ui.Controls;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class LoginWindow : FluentWindow
    {
        private readonly Random _random = new();

        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            viewModel.CloseAction = Close;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState =
                WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState =
                    WindowState == WindowState.Maximized
                        ? WindowState.Normal
                        : WindowState.Maximized;
            }
            else
            {
                DragMove();
            }
        }

        private void BookImagesBackground_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Image img &&
                img.RenderTransform is TranslateTransform transform)
            {
                double randomDistance = _random.Next(-25, -10);
                double randomDuration = _random.NextDouble() * 2 + 2;
                double randomDelay = _random.NextDouble() * 2;

                var animation = new DoubleAnimation
                {
                    To = randomDistance,
                    Duration = TimeSpan.FromSeconds(randomDuration),
                    BeginTime = TimeSpan.FromSeconds(randomDelay),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    EasingFunction = new SineEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    }
                };

                transform.BeginAnimation(
                    TranslateTransform.YProperty,
                    animation);
            }
        }
    }
}

