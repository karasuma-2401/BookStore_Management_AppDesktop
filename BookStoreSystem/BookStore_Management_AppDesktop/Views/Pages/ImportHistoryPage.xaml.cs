using BookStore_Management_AppDesktop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    public partial class ImportHistoryPage : Page
    {
        private readonly ImportHistoryViewModel _viewModel;

        public ImportHistoryPage(ImportHistoryViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            this.Loaded += ImportHistoryPage_Loaded;
        }

        private async void ImportHistoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadDataAsync();
        }

        private void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.DataContext != null)
            {
                if (_viewModel.ViewDetailCommand.CanExecute(row.DataContext))
                {
                    _viewModel.ViewDetailCommand.Execute(row.DataContext);
                }
            }
        }
    }
}