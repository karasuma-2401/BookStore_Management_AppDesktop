using BookStore_Management_AppDesktop.ViewModels;
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
    }
}
