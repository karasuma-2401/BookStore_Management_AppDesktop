using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class CustomMessageBox : Window
    {
        private MessageBoxResult _result = MessageBoxResult.None;

        public CustomMessageBox(string message, string title, MessageBoxButton button, MessageBoxImage icon)
        {
            InitializeComponent();
            txtMessage.Text = message;
            txtTitle.Text = title;

            ConfigureDialog(button, icon);
        }

        private void ConfigureDialog(MessageBoxButton button, MessageBoxImage icon)
        {
            // 1. Configure Icon Badge & Borders based on MessageBoxImage
            switch (icon)
            {
                case MessageBoxImage.Error:
                    BadgeIcon.Symbol = Wpf.Ui.Controls.SymbolRegular.DismissCircle24;
                    BadgeIcon.Foreground = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44)); // Rose red
                    BadgeBorder.Background = new SolidColorBrush(Color.FromRgb(0xFE, 0xE2, 0xE2));
                    BadgeBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFE, 0xCA, 0xCA));
                    CardBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFE, 0xCA, 0xCA));
                    CardShadow.Color = Color.FromRgb(0xEF, 0x44, 0x44);
                    CardShadow.Opacity = 0.08;

                    // Style Ok / Yes as red alert
                    BtnOk.Background = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44));
                    BtnYes.Background = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44));
                    break;

                case MessageBoxImage.Warning:
                    BadgeIcon.Symbol = Wpf.Ui.Controls.SymbolRegular.Warning24;
                    BadgeIcon.Foreground = new SolidColorBrush(Color.FromRgb(0xD9, 0x77, 0x06)); // Amber
                    BadgeBorder.Background = new SolidColorBrush(Color.FromRgb(0xFE, 0xF3, 0xC7));
                    BadgeBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFD, 0xE6, 0x8A));
                    CardBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFD, 0xE6, 0x8A));
                    CardShadow.Color = Color.FromRgb(0xD9, 0x77, 0x06);
                    CardShadow.Opacity = 0.06;

                    // Style Ok / Yes as amber
                    BtnOk.Background = new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B));
                    BtnYes.Background = new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B));
                    break;

                case MessageBoxImage.Question:
                    BadgeIcon.Symbol = Wpf.Ui.Controls.SymbolRegular.QuestionCircle24;
                    BadgeIcon.Foreground = new SolidColorBrush(Color.FromRgb(0x7C, 0x3A, 0xED)); // Violet
                    BadgeBorder.Background = new SolidColorBrush(Color.FromRgb(0xF5, 0xF3, 0xFF));
                    BadgeBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0xDD, 0xD6, 0xFE));
                    CardBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0xDD, 0xD6, 0xFE));
                    CardShadow.Color = Color.FromRgb(0x7C, 0x3A, 0xED);
                    CardShadow.Opacity = 0.06;

                    // Style Ok / Yes as violet
                    BtnOk.Background = new SolidColorBrush(Color.FromRgb(0x7C, 0x3A, 0xED));
                    BtnYes.Background = new SolidColorBrush(Color.FromRgb(0x7C, 0x3A, 0xED));
                    break;

                case MessageBoxImage.Information:
                    string titleLower = txtTitle.Text.ToLower();
                    if (titleLower.Contains("success") || titleLower.Contains("thành công") || titleLower.Contains("ok"))
                    {
                        // Success State
                        BadgeIcon.Symbol = Wpf.Ui.Controls.SymbolRegular.CheckmarkCircle24;
                        BadgeIcon.Foreground = new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81)); // Emerald Green
                        BadgeBorder.Background = new SolidColorBrush(Color.FromRgb(0xE0, 0xFD, 0xF9));
                        BadgeBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0x99, 0xF6, 0xE4));
                        CardBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0x99, 0xF6, 0xE4));
                        CardShadow.Color = Color.FromRgb(0x10, 0xB9, 0x81);
                        CardShadow.Opacity = 0.08;

                        BtnOk.Background = new SolidColorBrush(Color.FromRgb(0x00, 0xD4, 0xB2));
                        BtnYes.Background = new SolidColorBrush(Color.FromRgb(0x00, 0xD4, 0xB2));
                    }
                    else
                    {
                        // Info State
                        BadgeIcon.Symbol = Wpf.Ui.Controls.SymbolRegular.Info24;
                        BadgeIcon.Foreground = new SolidColorBrush(Color.FromRgb(0x25, 0x63, 0xEB)); // Blue
                        BadgeBorder.Background = new SolidColorBrush(Color.FromRgb(0xEF, 0xF6, 0xFF));
                        BadgeBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0xBF, 0xDB, 0xFE));
                        CardBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0xBF, 0xDB, 0xFE));
                        CardShadow.Color = Color.FromRgb(0x25, 0x63, 0xEB);
                        CardShadow.Opacity = 0.06;

                        BtnOk.Background = new SolidColorBrush(Color.FromRgb(0x0E, 0xA5, 0xE9));
                        BtnYes.Background = new SolidColorBrush(Color.FromRgb(0x0E, 0xA5, 0xE9));
                    }
                    break;

                default:
                    // Default or none: Info style
                    BadgeIcon.Symbol = Wpf.Ui.Controls.SymbolRegular.Info24;
                    BadgeIcon.Foreground = new SolidColorBrush(Color.FromRgb(0x25, 0x63, 0xEB));
                    BadgeBorder.Background = new SolidColorBrush(Color.FromRgb(0xEF, 0xF6, 0xFF));
                    BadgeBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0xBF, 0xDB, 0xFE));
                    CardBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0xBF, 0xDB, 0xFE));

                    BtnOk.Background = new SolidColorBrush(Color.FromRgb(0x0E, 0xA5, 0xE9));
                    BtnYes.Background = new SolidColorBrush(Color.FromRgb(0x0E, 0xA5, 0xE9));
                    break;
            }

            // 2. Configure Buttons panel based on MessageBoxButton
            switch (button)
            {
                case MessageBoxButton.OK:
                    BtnOk.Visibility = Visibility.Visible;
                    BtnCancel.Visibility = Visibility.Collapsed;
                    BtnYes.Visibility = Visibility.Collapsed;
                    BtnNo.Visibility = Visibility.Collapsed;
                    break;

                case MessageBoxButton.OKCancel:
                    BtnOk.Visibility = Visibility.Visible;
                    BtnCancel.Visibility = Visibility.Visible;
                    BtnYes.Visibility = Visibility.Collapsed;
                    BtnNo.Visibility = Visibility.Collapsed;
                    break;

                case MessageBoxButton.YesNo:
                    BtnOk.Visibility = Visibility.Collapsed;
                    BtnCancel.Visibility = Visibility.Collapsed;
                    BtnYes.Visibility = Visibility.Visible;
                    BtnNo.Visibility = Visibility.Visible;
                    break;

                case MessageBoxButton.YesNoCancel:
                    BtnOk.Visibility = Visibility.Collapsed;
                    BtnCancel.Visibility = Visibility.Visible;
                    BtnYes.Visibility = Visibility.Visible;
                    BtnNo.Visibility = Visibility.Visible;
                    break;
            }
        }

        public static MessageBoxResult Show(string message, string title = "Notification", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None)
        {
            MessageBoxResult result = MessageBoxResult.None;

            if (Application.Current.Dispatcher.CheckAccess())
            {
                var msgBox = new CustomMessageBox(message, title, button, icon)
                {
                    Owner = Application.Current.MainWindow
                };
                msgBox.ShowDialog();
                result = msgBox._result;
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var msgBox = new CustomMessageBox(message, title, button, icon)
                    {
                        Owner = Application.Current.MainWindow
                    };
                    msgBox.ShowDialog();
                    result = msgBox._result;
                });
            }

            return result;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Cancel;
            this.DialogResult = false;
            this.Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.OK;
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Cancel;
            this.DialogResult = false;
            this.Close();
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Yes;
            this.DialogResult = true;
            this.Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.No;
            this.DialogResult = false;
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
