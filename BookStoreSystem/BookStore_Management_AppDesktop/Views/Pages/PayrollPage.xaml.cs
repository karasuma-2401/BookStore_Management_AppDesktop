using System.Windows.Controls;
using BookStore_Management_AppDesktop.ViewModels;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for PayrollPage.xaml
    /// </summary>
    public partial class PayrollPage : Page
    {
        public PayrollPage(PayrollViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
