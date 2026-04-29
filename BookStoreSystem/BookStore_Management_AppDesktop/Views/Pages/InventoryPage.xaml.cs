using BookStore_Management_AppDesktop.ViewModels;
using BookStore_Management_AppDesktop.Views.Windows;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore_Management_AppDesktop.Views.Pages
{

    public partial class InventoryPage : Page
    {
        private bool _isDataLoaded = false;
        public InventoryPage()
        {
            InitializeComponent();

            var viewModel = App.ServiceProvider!.GetRequiredService<InventoryViewModel>();
            this.DataContext = viewModel;
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isDataLoaded) return;

            if (this.DataContext is InventoryViewModel viewModel)
            {
                await viewModel.LoadDataAsync();
                _isDataLoaded = true;
            }
        }

    }
}