using BookStore_Management_AppDesktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class BookViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Book> _books;

        public BookViewModel()
        {
            Books = new ObservableCollection<Book>();
            LoadFakeData();
        }

        private void LoadFakeData()
        {
            
        }
    }
}