using BookStore_Management_AppDesktop.ViewModels;
using System.Windows.Controls;


namespace BookStore_Management_AppDesktop.Views.Pages
{

    public partial class ImportHistoryPage : Page
    {
        public ImportHistoryPage(ImportHistoryViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel; 
        }
    }
}
