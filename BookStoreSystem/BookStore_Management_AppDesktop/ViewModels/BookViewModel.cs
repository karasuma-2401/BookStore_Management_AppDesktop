using BookStore_Management_AppDesktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class BookViewModel : ObservableObject
    {
        // Danh sách các cuốn sách. Toolkit sẽ tự sinh ra thuộc tính public "Books"
        [ObservableProperty]
        private ObservableCollection<Book> _books;

        public BookViewModel()
        {
            Books = new ObservableCollection<Book>();
            LoadFakeData();
        }

        private void LoadFakeData()
        {
            // Sử dụng ảnh bìa từ OpenLibrary (Dùng mã ISBN để lấy ảnh chất lượng cao)
            _books.Add(new Book
            {
                Title = "The Nightingale: A Novel",
                Author = "Kristin Hannah",
                Price = "$14.99",
                CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780312577220-L.jpg"
            });

            _books.Add(new Book
            {
                Title = "Atomic Habits",
                Author = "James Clear",
                Price = "$11.98",
                CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780735211292-L.jpg"
            });

            _books.Add(new Book
            {
                Title = "1984 (Signet Classics)",
                Author = "George Orwell",
                Price = "$9.99",
                CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780451524935-L.jpg"
            });

            _books.Add(new Book
            {
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                Price = "$8.74",
                CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780743273565-L.jpg"
            });

            _books.Add(new Book
            {
                Title = "Sapiens: A Brief History of Humankind",
                Author = "Yuval Noah Harari",
                Price = "$15.48",
                CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780062316097-L.jpg"
            });

            _books.Add(new Book
            {
                Title = "Dune (Dune Chronicles, Book 1)",
                Author = "Frank Herbert",
                Price = "$10.50",
                CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780441172719-L.jpg"
            });
        }
    }
}