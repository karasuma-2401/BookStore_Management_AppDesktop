using BookStore_Management_AppDesktop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        private void ImportHistoryGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep is DataGridRow row && row.DataContext is ImportHistoryUIModel importData)
            {
                if (_viewModel.ViewDetailCommand.CanExecute(importData))
                {
                    _viewModel.ViewDetailCommand.Execute(importData);
                }
            }
        }
    }
}