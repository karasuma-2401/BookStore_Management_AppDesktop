using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class ConfirmWindow : Window
    {
        public ConfirmWindow(string message, string confirmText = "Confirm", bool isDanger = false)
        {
            InitializeComponent();

            txtMessage.Text = message;
            btnConfirm.Content = confirmText;

            if (isDanger)
            {
                DangerIcon.Visibility = Visibility.Visible;
                InfoIcon.Visibility = Visibility.Collapsed;
                btnConfirm.Background = (SolidColorBrush)FindResource("ErrorPrimaryBrush");
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
