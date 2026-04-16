using BookStore_Management_AppDesktop.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class EditEmployeeWindow : Window
    {
        public EditEmployeeWindow(UpdateEmployeeViewModel viewModel)
        {
            InitializeComponent();

            // Set the DataContext
            this.DataContext = viewModel;

            // Handle Message Requests from ViewModel
            viewModel.OnShowMessage = (title, message) =>
            {
                MessageBoxImage icon = title.Contains("Error") ? MessageBoxImage.Error : MessageBoxImage.Information;
                MessageBox.Show(message, title, MessageBoxButton.OK, icon);
            };

            // Handle Close Requests from ViewModel
            viewModel.OnRequestClose = (dialogResult) =>
            {
                this.DialogResult = dialogResult;
                this.Close();
            };
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}