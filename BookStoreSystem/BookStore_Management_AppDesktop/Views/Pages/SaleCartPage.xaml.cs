using BookStore_Management_AppDesktop.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for SaleCartPage.xaml
    /// </summary>
    public partial class SaleCartPage : Page
    {
        public SaleCartPage(SaleCartViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is SaleCartViewModel viewModel)
            {
                await viewModel.LoadDataAsync();
            }
        }
    }
}
