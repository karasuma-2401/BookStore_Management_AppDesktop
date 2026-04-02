using BookStore_Management_AppDesktop.Models; 
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32; 
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class AddBookViewModel : ObservableObject
    {
        private readonly IBookApiService _bookApiService;
        private readonly CloudinaryService _cloudinaryService;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private int? _authorId;

        [ObservableProperty]
        private int _quantity;

        [ObservableProperty]
        private string _localImagePath = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;

        public AddBookViewModel()
        {
            _bookApiService = new BookApiService();
            _cloudinaryService = new CloudinaryService();
        }

        public Action<string>? OnShowMessage { get; set; }
        public Action? OnRequestClose { get; set; }

        [RelayCommand]
        private void SelectImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.webp",
                Title = "Chọn ảnh bìa sách"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LocalImagePath = openFileDialog.FileName;
            }
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            window?.Close();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                OnShowMessage?.Invoke("Please enter the book title!");
                return;
            }

            try
            {
                IsLoading = true;

                string imageUrl = string.Empty;

                if (!string.IsNullOrEmpty(LocalImagePath))
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(LocalImagePath);
                }

                var newBook = new Book
                {
                    Title = Title,
                    AuthorId = AuthorId,
                    Quantity = Quantity,
                    ImagePath = imageUrl
                };

                bool isSuccess = await _bookApiService.CreateBookAsync(newBook);

                if (isSuccess)
                {
                    OnShowMessage?.Invoke("Book added successfully!");
                    OnRequestClose?.Invoke(); 
                }
                else
                {
                    OnShowMessage?.Invoke("Added failed. Please recheck!");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}