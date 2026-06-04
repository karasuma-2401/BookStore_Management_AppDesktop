using System;
using System.Windows;
using MessageBox = BookStore_Management_AppDesktop.Views.Windows.CustomMessageBox;
using System.Windows.Input;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class PaymentDialog : Window
    {
        public decimal PaymentAmount { get; private set; }

        public PaymentDialog(decimal defaultAmount = 0)
        {
            InitializeComponent();
            AmountTextBox.Text = defaultAmount > 0 ? defaultAmount.ToString("F0") : string.Empty;
            AmountTextBox.Focus();
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
            if (decimal.TryParse(AmountTextBox.Text, out decimal amount) && amount > 0)
            {
                PaymentAmount = amount;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid positive payment amount.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
