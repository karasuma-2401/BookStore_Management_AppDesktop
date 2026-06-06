using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace BookStore_Management_AppDesktop.Views.Windows
{

    public partial class EditBookWindow : Window
    {

        public EditBookWindow(object viewModelContext)
        {
            InitializeComponent();

            this.DataContext = viewModelContext;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Chỉ cho phép nhập ký tự số vào ô Publish Year.
        /// </summary>
        private void PublishYearTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        /// <summary>
        /// Ngăn chặn dán nội dung không phải số vào ô Publish Year.
        /// </summary>
        private void PublishYearTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetData(typeof(string)) is string pastedText && !IsTextNumeric(pastedText))
            {
                e.CancelCommand();
            }
        }

        private static bool IsTextNumeric(string text)
        {
            return !string.IsNullOrEmpty(text) && Regex.IsMatch(text, "^[0-9]+$");
        }
    }
}
