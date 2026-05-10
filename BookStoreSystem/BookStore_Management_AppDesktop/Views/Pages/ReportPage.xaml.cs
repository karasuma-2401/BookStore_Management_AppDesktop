using BookStore_Management_AppDesktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    public partial class ReportPage : Page
    {
        private bool _isDataLoaded = false;

        public ReportPage()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider!.GetRequiredService<ReportViewModel>();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isDataLoaded) return;

            if (this.DataContext is ReportViewModel viewModel)
            {
                await viewModel.LoadDataAsync();
                _isDataLoaded = true;
            }
        }
    }
}