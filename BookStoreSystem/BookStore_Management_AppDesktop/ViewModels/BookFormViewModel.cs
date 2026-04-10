using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class BookFormViewModel : ObservableObject
    {
        // Khai báo giao diện (Interfaces) thay vì class cụ thể để dễ dàng Unit Test sau này
        private readonly IBookApiService _bookApiService;
        private readonly CloudinaryService _cloudinaryService;

        // [KIẾN TRÚC COMPOSITION]: Khối Lego Quản lý Tác giả
        public AuthorSelectionViewModel AuthorVM { get; }

        private readonly bool _isEditMode;
        private readonly int _bookId;
        private readonly int? _initialAuthorId;

        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private int _quantity;
        [ObservableProperty] private string _localImagePath = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private bool _isLoading;

        public Action<string>? OnShowMessage { get; set; }
        public Action? OnRequestClose { get; set; }

        // [TỐI ƯU 2.2 - DEPENDENCY INJECTION]: Không dùng "new" bên trong class nữa
        // Gộp chung 2 Constructor làm 1, Book truyền vào là null thì hiểu là Add, có Book thì là Edit
        public BookFormViewModel(
            IBookApiService bookApiService,
            CloudinaryService cloudinaryService,
            AuthorSelectionViewModel authorVM,
            Book? bookToEdit = null)
        {
            _bookApiService = bookApiService;
            _cloudinaryService = cloudinaryService;
            AuthorVM = authorVM;

            // Ràng buộc sự kiện thông báo của AuthorVM chung với Form này
            AuthorVM.OnShowMessage = (msg) => OnShowMessage?.Invoke(msg);

            if (bookToEdit == null)
            {
                _isEditMode = false;
            }
            else
            {
                _isEditMode = true;
                _bookId = bookToEdit.BookId;
                Title = bookToEdit.Title;
                Quantity = bookToEdit.Quantity;
                LocalImagePath = bookToEdit.ImagePath ?? string.Empty;
                _initialAuthorId = bookToEdit.AuthorId;
            }
        }

        public async Task InitializeAsync()
        {
            // Kích hoạt khối Lego Tác giả, truyền ID cũ (nếu có) để nó tự tìm và Select
            await AuthorVM.InitializeAsync(_initialAuthorId);
        }

        private bool CanExecuteAction() => !IsLoading;

        [RelayCommand]
        private void SelectImage()
        {
            var dialog = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.webp" };
            if (dialog.ShowDialog() != true) return;

            var file = new FileInfo(dialog.FileName);
            if (!file.Exists) { OnShowMessage?.Invoke("File not found."); return; }
            if (file.Length > 5 * 1024 * 1024) { OnShowMessage?.Invoke("Max 5MB."); return; }

            LocalImagePath = file.FullName;
        }

        // [TỐI ƯU 2.3 - THUẦN MVVM]: Bỏ tham số Window, giao diện tự đóng thông qua Action
        [RelayCommand]
        private void Cancel() => OnRequestClose?.Invoke();

        [RelayCommand(CanExecute = nameof(CanExecuteAction))]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Title)) { OnShowMessage?.Invoke("Enter title."); return; }

            // Lấy ID từ khối Lego Tác giả
            var selectedAuthorId = AuthorVM.SelectedAuthor?.AuthorId;
            if (selectedAuthorId is null or 0) { OnShowMessage?.Invoke("Select author."); return; }

            if (Quantity <= 0) { OnShowMessage?.Invoke("Quantity > 0."); return; }

            try
            {
                IsLoading = true;
                string finalImageUrl = LocalImagePath;

                // [TỐI ƯU 2.1 - URL VALIDATION]: Dùng Uri.TryCreate để check chính xác
                bool isHttpLink = Uri.TryCreate(LocalImagePath, UriKind.Absolute, out var uri) &&
                                  (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

                if (!string.IsNullOrEmpty(LocalImagePath) && !isHttpLink)
                {
                    try
                    {
                        finalImageUrl = await _cloudinaryService.UploadImageAsync(LocalImagePath);
                    }
                    // [TỐI ƯU 2.4 - EXCEPTION]: Log lỗi ra Debug để dev truy vết
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[Cloudinary Error]: {ex}");
                        OnShowMessage?.Invoke("Upload failed.");
                        return;
                    }
                }

                var book = new Book
                {
                    BookId = _isEditMode ? _bookId : 0,
                    Title = Title.Trim(),
                    AuthorId = selectedAuthorId,
                    Quantity = Quantity,
                    ImagePath = finalImageUrl
                };

                try
                {
                    bool ok = _isEditMode
                        ? await _bookApiService.UpdateBookAsync(_bookId, book)
                        : await _bookApiService.CreateBookAsync(book);

                    if (!ok) { OnShowMessage?.Invoke("Save failed."); return; }

                    OnShowMessage?.Invoke("Success.");
                    OnRequestClose?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[API Error]: {ex}");
                    OnShowMessage?.Invoke("Save failed.");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}