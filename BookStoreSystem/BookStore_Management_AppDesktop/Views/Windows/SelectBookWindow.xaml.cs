using BookStore_Management_AppDesktop.ViewModels;
using System.Windows;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class SelectBookWindow : Window
    {
        public SelectBookWindow(SelectBookViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectBookViewModel.SearchKeyword) && string.IsNullOrEmpty(viewModel.SearchKeyword))
                {
                    SearchBox.Focus();
                }
            };
        }
    }
}