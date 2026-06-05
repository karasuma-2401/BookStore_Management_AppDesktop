using BookStore_Management_AppDesktop.ViewModels;
using System.Windows;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class AddBookWindow : Window
    {
        public AddBookWindow(BookFormViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            viewModel.OnRequestClose = () =>
            {
                this.Close();
            };
        }
    }
}