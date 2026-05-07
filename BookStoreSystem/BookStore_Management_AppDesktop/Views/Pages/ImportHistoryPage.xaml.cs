using BookStore_Management_AppDesktop.ViewModels;
using System.Windows;
using System.Windows.Controls;


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
    }
}
