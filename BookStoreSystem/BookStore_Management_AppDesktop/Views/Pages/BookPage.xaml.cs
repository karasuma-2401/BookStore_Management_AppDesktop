
using System.Windows.Controls;
using BookStore_Management_AppDesktop.ViewModels;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for BookPage.xaml
    /// </summary>
    public partial class BookPage : Page
    {
        public BookPage()
        {
            InitializeComponent();

            // Gán ViewModel giả để test giao diện Amazon lên hình
            this.DataContext = new BookViewModel();
        }
    }
}
