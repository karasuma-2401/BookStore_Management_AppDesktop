using System;
using System.Windows;
using System.Windows.Input;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class ChangeEmployeePasswordDialog : Window
    {
        public string NewPassword { get; private set; } = string.Empty;

        public ChangeEmployeePasswordDialog(string employeeName)
        {
            InitializeComponent();
            EmployeeNameTextBlock.Text = $"Change Password for: {employeeName}";
            NewPasswordBox.Focus();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            var pwd = NewPasswordBox.Password;
            if (string.IsNullOrWhiteSpace(pwd))
            {
                MessageBox.Show("Password cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (pwd.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewPassword = pwd;
            this.DialogResult = true;
            this.Close();
        }
    }
}
