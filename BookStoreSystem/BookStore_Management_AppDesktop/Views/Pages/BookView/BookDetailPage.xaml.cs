using System.Windows.Controls;
using BookStore_Management_AppDesktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    public partial class BookDetailPage : Page
    {
        public BookDetailPage()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider!.GetRequiredService<BookDetailViewModel>();
        }
    }
}