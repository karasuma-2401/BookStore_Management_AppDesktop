using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using BookStore_Management_AppDesktop.Messages;
using BookStore_Management_AppDesktop.Services.API.Book;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class BookFormViewModel : ObservableObject
    {
        private readonly IBookApiService _bookApiService;
        private readonly CloudinaryService _cloudinaryService;

        public AuthorSelectionViewModel AuthorVM { get; }

        private bool _isEditMode;
        private int _bookId;
        private int? _initialAuthorId;

        // "HẠT SẠN" ĐÃ FIX: Biến lưu trữ số lượng hiện có của sách (chỉ dùng nội bộ, không show UI)
        private int _originalQuantity = 0;

        [ObservableProperty] private string _title = string.Empty;

        // ĐÃ XÓA: [ObservableProperty] private int _quantity; (Vì không cho phép sửa tay nữa)

        [ObservableProperty] private string _localImagePath = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private bool _isLoading;

        public Action<string>? OnShowMessage { get; set; }
        public Action? OnRequestClose { get; set; }

        public BookFormViewModel(
            IBookApiService bookApiService,
            CloudinaryService cloudinaryService,
            AuthorSelectionViewModel authorVM)
        {
            _bookApiService = bookApiService;
            _cloudinaryService = cloudinaryService;
            AuthorVM = authorVM;

            AuthorVM.OnShowMessage = (msg) => OnShowMessage?.Invoke(msg);
            _isEditMode = false;
        }

        public void SetupEditMode(Book bookToEdit)
        {
            _isEditMode = true;
            _bookId = bookToEdit.BookId;
            Title = bookToEdit.Title ?? string.Empty;
            LocalImagePath = bookToEdit.ImagePath ?? string.Empty;
            _initialAuthorId = bookToEdit.AuthorId;

            // LƯU LẠI số lượng hiện có để khi gửi Messenger quay lại Inventory, 
            // số lượng trên lưới không bị biến thành 0.
            _originalQuantity = bookToEdit.Quantity;
        }

        public async Task InitializeAsync()
        {
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

        [RelayCommand]
        private void Cancel() => OnRequestClose?.Invoke();

        [RelayCommand(CanExecute = nameof(CanExecuteAction))]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Title)) { OnShowMessage?.Invoke("Enter title."); return; }

            var selectedAuthorId = AuthorVM.SelectedAuthor?.AuthorId;
            if (selectedAuthorId is null or 0) { OnShowMessage?.Invoke("Select author."); return; }

            // ĐÃ XÓA: Validation check Quantity <= 0 (Vì UI không có ô nhập nữa)

            try
            {
                IsLoading = true;
                string finalImageUrl = LocalImagePath;

                bool isHttpLink = Uri.TryCreate(LocalImagePath, UriKind.Absolute, out var uri) &&
                                  (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

                if (!string.IsNullOrEmpty(LocalImagePath) && !isHttpLink)
                {
                    try
                    {
                        finalImageUrl = await _cloudinaryService.UploadImageAsync(LocalImagePath);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Cloudinary Error]: {ex}");
                        OnShowMessage?.Invoke("Upload failed.");
                        return;
                    }
                }

                var book = new Book
                {
                    BookId = _isEditMode ? _bookId : 0,
                    Title = Title.Trim(),
                    AuthorId = selectedAuthorId,
                    // QUY TẮC MỚI: 
                    // Nếu thêm mới -> Quantity mặc định là 0.
                    // Nếu sửa -> Trả lại số lượng cũ.
                    Quantity = _isEditMode ? _originalQuantity : 0,
                    ImagePath = finalImageUrl
                };

                try
                {
                    if (_isEditMode)
                    {
                        bool isUpdated = await _bookApiService.UpdateBookAsync(_bookId, book);
                        if (!isUpdated)
                        {
                            OnShowMessage?.Invoke("Update failed.");
                            return;
                        }

                        WeakReferenceMessenger.Default.Send(new BookChangedMessage(BookChangedMessage.ChangeAction.Update, book));
                    }
                    else
                    {
                        var createdBook = await _bookApiService.CreateBookAsync(book);
                        if (createdBook == null)
                        {
                            OnShowMessage?.Invoke("Create failed.");
                            return;
                        }

                        WeakReferenceMessenger.Default.Send(new BookChangedMessage(BookChangedMessage.ChangeAction.Add, createdBook));
                    }

                    OnShowMessage?.Invoke("Success.");
                    OnRequestClose?.Invoke();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[API Error]: {ex}");
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