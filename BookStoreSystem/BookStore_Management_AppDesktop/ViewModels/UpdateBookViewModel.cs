using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class EditBookViewModel : ObservableObject
    {
        private readonly IBookApiService _bookApiService;
        private readonly CloudinaryService _cloudinaryService;

        private readonly int _bookId;

        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private int? _authorId;
        [ObservableProperty] private int _quantity;
        [ObservableProperty] private string _localImagePath = string.Empty;
        [ObservableProperty] private bool _isLoading = false;


        public Action<string>? OnShowMessage { get; set; }
        public Action? OnRequestClose { get; set; }

        public EditBookViewModel(Book bookToEdit)
        {
            _bookApiService = new BookApiService();
            _cloudinaryService = new CloudinaryService();

            _bookId = bookToEdit.BookId;
            Title = bookToEdit.Title;
            AuthorId = bookToEdit.AuthorId;
            Quantity = bookToEdit.Quantity;

            LocalImagePath = bookToEdit.ImagePath;
        }

        [RelayCommand]
        private void SelectImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.webp",
                Title = "Chọn ảnh bìa sách thay thế"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LocalImagePath = openFileDialog.FileName;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            OnRequestClose?.Invoke();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                OnShowMessage?.Invoke("Vui lòng nhập tên sách!");
                return;
            }

            try
            {
                IsLoading = true;

                string finalImageUrl = LocalImagePath;

                if (!string.IsNullOrEmpty(LocalImagePath) && !LocalImagePath.StartsWith("http"))
                {
                    finalImageUrl = await _cloudinaryService.UploadImageAsync(LocalImagePath);
                }

                var updatedBook = new Book
                {
                    BookId = _bookId,
                    Title = Title,
                    AuthorId = AuthorId,
                    Quantity = Quantity,
                    ImagePath = finalImageUrl
                };

                bool isSuccess = await _bookApiService.UpdateBookAsync(_bookId, updatedBook);

                if (isSuccess)
                {
                    OnShowMessage?.Invoke("Cập nhật thông tin sách thành công!");
                    OnRequestClose?.Invoke();
                }
                else
                {
                    OnShowMessage?.Invoke("Cập nhật thất bại. Vui lòng kiểm tra lại!");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
