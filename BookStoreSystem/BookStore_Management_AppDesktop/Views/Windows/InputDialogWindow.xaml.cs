using System.Windows;
using System.Windows.Controls; 

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

        private void InputTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
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