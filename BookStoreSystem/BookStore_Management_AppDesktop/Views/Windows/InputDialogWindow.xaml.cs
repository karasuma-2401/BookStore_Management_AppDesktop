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
    public partial class InputDialogWindow : Window
    {
        public string InputText => InputTxt.Text;

        public InputDialogWindow(string title, string message, string defaultText = "")
        {
            InitializeComponent();
            Title = title;
            MessageLabel.Text = message;
            InputTxt.Text = defaultText;
            InputTxt.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputTxt.Text))
            {
                System.Windows.MessageBox.Show("Value cannot be empty!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }

}
