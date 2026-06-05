using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.BookServices;
using BookStore_Management_AppDesktop.Services.API.AuthorServices;
using BookStore_Management_AppDesktop.Services.API.CategoryServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class BookFormViewModel : ObservableObject
    {
        private readonly IBookApiService _bookApiService;
        private readonly IAuthorApiService _authorApiService;
        private readonly ICategoryApiService _categoryApiService;
        private readonly CloudinaryService _cloudinaryService;

        public Action<string>? OnShowMessage { get; set; }
        public Action? OnRequestClose { get; set; }
        public Action? OnSaveSuccess { get; set; }

        private bool _isEditMode;
        [ObservableProperty] private int _bookId;

        // 🎯 CÁC TRƯỜNG DỮ LIỆU CỦA SÁCH
        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private int _publishYear = DateTime.Now.Year;
        [ObservableProperty] private decimal _price = 0;
        [ObservableProperty] private int _quantity = 0;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private string _localImagePath = string.Empty;

        [ObservableProperty][NotifyCanExecuteChangedFor(nameof(SaveBookChangesCommand))] private bool _isLoading;

        // 🎯 CÁC TRƯỜNG PHỤC VỤ AUTO-COMPLETE (CHIPS)
        private List<Author> _allFetchedAuthors = new();
        private List<Category> _allFetchedCategories = new();

        [ObservableProperty] private ObservableCollection<Author> _availableAuthors = new();
        [ObservableProperty] private ObservableCollection<Category> _availableCategories = new();
        [ObservableProperty] private ObservableCollection<Author> _selectedBookAuthors = new();
        [ObservableProperty] private ObservableCollection<Category> _selectedBookCategories = new();

        [ObservableProperty] private string _authorSearchText = string.Empty;
        [ObservableProperty] private string _categorySearchText = string.Empty;
        [ObservableProperty] private Author? _selectedAuthorComboItem;
        [ObservableProperty] private Category? _selectedCategoryComboItem;
        [ObservableProperty] private bool _isAuthorDropdownOpen;
        [ObservableProperty] private bool _isCategoryDropdownOpen;

        public BookFormViewModel(
            IBookApiService bookApiService,
            IAuthorApiService authorApiService,
            ICategoryApiService categoryApiService,
            CloudinaryService cloudinaryService)
        {
            _bookApiService = bookApiService;
            _authorApiService = authorApiService;
            _categoryApiService = categoryApiService;
            _cloudinaryService = cloudinaryService;
        }

        // ==========================================
        // 🎯 HÀM SETUP CHẾ ĐỘ ADD HOẶC EDIT
        // ==========================================
        public async Task SetupAddModeAsync()
        {
            _isEditMode = false;
            Title = string.Empty;
            Description = string.Empty;
            LocalImagePath = string.Empty;
            Price = 0;     // Sách mới chưa có giá (đợi Import)
            Quantity = 0;  // Sách mới tồn kho = 0 (đợi Import)
            SelectedBookAuthors.Clear();
            SelectedBookCategories.Clear();

            await LoadLookupsAsync();
        }

        public async Task SetupEditModeAsync(Book bookToEdit)
        {
            _isEditMode = true;
            _bookId = bookToEdit.BookId;
            Title = bookToEdit.Title ?? string.Empty;
            PublishYear = bookToEdit.PublishYear;
            Price = bookToEdit.Price;
            Quantity = bookToEdit.Quantity;
            Description = bookToEdit.Description ?? string.Empty;
            LocalImagePath = bookToEdit.ImagePath ?? string.Empty;

            await LoadLookupsAsync();

            SelectedBookAuthors = new ObservableCollection<Author>(
                bookToEdit.AuthorNames.Select((name, idx) => new Author { AuthorId = bookToEdit.AuthorIds.ElementAtOrDefault(idx), Name = name }));

            SelectedBookCategories = new ObservableCollection<Category>(
                bookToEdit.CategoryNames.Select((name, idx) => new Category { CategoryId = bookToEdit.CategoryIds.ElementAtOrDefault(idx), Name = name }));
        }

        private async Task LoadLookupsAsync()
        {
            try
            {
                var authorsFromApi = await _authorApiService.GetAllAuthorsAsync();
                var categoriesFromApi = await _categoryApiService.GetAllCategoriesAsync();

                _allFetchedAuthors = authorsFromApi?.ToList() ?? new List<Author>();
                _allFetchedCategories = categoriesFromApi?.ToList() ?? new List<Category>();

                AvailableAuthors = new ObservableCollection<Author>(_allFetchedAuthors);
                AvailableCategories = new ObservableCollection<Category>(_allFetchedCategories);
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Error loading lookups]: {ex.Message}"); }
        }

        // ==========================================
        // 🎯 LOGIC UPLOAD ẢNH & SAVE
        // ==========================================
        [RelayCommand]
        private void Cancel() => OnRequestClose?.Invoke();

        [RelayCommand]
        private async Task ChangeBookImageAsync()
        {
            var dialog = new OpenFileDialog { Title = "Select Book Cover Image", Filter = "Image Files|*.jpg;*.jpeg;*.png;*.webp", CheckFileExists = true };
            if (dialog.ShowDialog() == true)
            {
                LocalImagePath = dialog.FileName;
            }
        }

        private bool CanExecuteSave() => !IsLoading;

        [RelayCommand(CanExecute = nameof(CanExecuteSave))]
        private async Task SaveBookChangesAsync()
        {
            if (string.IsNullOrWhiteSpace(Title)) { OnShowMessage?.Invoke("Please enter book title."); return; }
            if (!SelectedBookAuthors.Any()) { OnShowMessage?.Invoke("Please select at least one author."); return; }
            if (!SelectedBookCategories.Any()) { OnShowMessage?.Invoke("Please select at least one category."); return; }

            int currentYear = DateTime.Now.Year;
            if (PublishYear <= 1445) { OnShowMessage?.Invoke($"Publish year must be greater than 1445."); return; }
            if (PublishYear > currentYear) { OnShowMessage?.Invoke($"Publish year cannot be greater than {currentYear}."); return; }

            try
            {
                IsLoading = true;
                string finalImageUrl = LocalImagePath;

                bool isHttpLink = Uri.TryCreate(LocalImagePath, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
                if (!string.IsNullOrEmpty(LocalImagePath) && !isHttpLink)
                {
                    OnShowMessage?.Invoke("Uploading image... Please wait.");
                    finalImageUrl = await _cloudinaryService.UploadImageAsync(LocalImagePath);
                    if (string.IsNullOrEmpty(finalImageUrl)) { OnShowMessage?.Invoke("Image upload failed."); return; }
                }

                var bookData = new Book
                {
                    BookId = _isEditMode ? _bookId : 0,
                    Title = Title.Trim(),
                    PublishYear = PublishYear,
                    Quantity = Quantity,
                    Price = Price,
                    Description = Description.Trim(),
                    ImagePath = finalImageUrl,
                    AuthorIds = SelectedBookAuthors.Select(a => a.AuthorId).ToList(),
                    CategoryIds = SelectedBookCategories.Select(c => c.CategoryId).ToList()
                };

                bool isSuccess;
                if (_isEditMode)
                {
                    isSuccess = await _bookApiService.UpdateBookAsync(_bookId, bookData);
                }
                else
                {
                    var createdBook = await _bookApiService.CreateBookAsync(bookData);
                    isSuccess = createdBook != null;
                }

                if (isSuccess)
                {
                    OnShowMessage?.Invoke(_isEditMode ? "Book updated successfully!" : "Book created successfully!");
                    OnSaveSuccess?.Invoke();
                    OnRequestClose?.Invoke();
                }
                else
                {
                    OnShowMessage?.Invoke("Operation failed. Please check connection.");
                }
            }
            catch (Exception ex) { OnShowMessage?.Invoke("An error occurred during save."); }
            finally { IsLoading = false; }
        }

        // ========================================================
        // 🎯 LOGIC TÁC GIẢ & THỂ LOẠI (AUTO-COMPLETE & ADD MỚI)
        // ========================================================
        partial void OnAuthorSearchTextChanged(string value)
        {
            AvailableAuthors.Clear();
            var keyword = value?.Trim() ?? string.Empty;
            var filtered = string.IsNullOrEmpty(keyword) ? _allFetchedAuthors : _allFetchedAuthors.Where(a => a.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            foreach (var a in filtered) AvailableAuthors.Add(a);

            if (!string.IsNullOrEmpty(keyword))
            {
                if (!_allFetchedAuthors.Any(a => a.Name.Equals(keyword, StringComparison.OrdinalIgnoreCase)))
                    AvailableAuthors.Add(new Author { AuthorId = -1, Name = $"Add \"{keyword}\" to database..." });
                IsAuthorDropdownOpen = true;
            }
            else IsAuthorDropdownOpen = AvailableAuthors.Any();
        }

        partial void OnSelectedAuthorComboItemChanged(Author? value)
        {
            if (value == null) return;
            System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                if (value.AuthorId == -1)
                {
                    var saved = await _authorApiService.CreateAuthorAsync(new Author { Name = AuthorSearchText.Trim() });
                    if (saved != null) { _allFetchedAuthors.Add(saved); if (!SelectedBookAuthors.Any(a => a.AuthorId == saved.AuthorId)) SelectedBookAuthors.Add(saved); }
                }
                else if (!SelectedBookAuthors.Any(a => a.AuthorId == value.AuthorId)) SelectedBookAuthors.Add(value);

                AuthorSearchText = string.Empty; IsAuthorDropdownOpen = false; SelectedAuthorComboItem = null;
            });
        }
        [RelayCommand] private void RemoveAuthor(Author author) => SelectedBookAuthors.Remove(author);


        partial void OnCategorySearchTextChanged(string value)
        {
            AvailableCategories.Clear();
            var keyword = value?.Trim() ?? string.Empty;
            var filtered = string.IsNullOrEmpty(keyword) ? _allFetchedCategories : _allFetchedCategories.Where(c => c.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            foreach (var c in filtered) AvailableCategories.Add(c);

            if (!string.IsNullOrEmpty(keyword))
            {
                if (!_allFetchedCategories.Any(c => c.Name.Equals(keyword, StringComparison.OrdinalIgnoreCase)))
                    AvailableCategories.Add(new Category { CategoryId = -1, Name = $"Add \"{keyword}\" to database..." });
                IsCategoryDropdownOpen = true;
            }
            else IsCategoryDropdownOpen = AvailableCategories.Any();
        }

        partial void OnSelectedCategoryComboItemChanged(Category? value)
        {
            if (value == null) return;
            System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                if (value.CategoryId == -1)
                {
                    var saved = await _categoryApiService.CreateCategoryAsync(new Category { Name = CategorySearchText.Trim() });
                    if (saved != null) { _allFetchedCategories.Add(saved); if (!SelectedBookCategories.Any(c => c.CategoryId == saved.CategoryId)) SelectedBookCategories.Add(saved); }
                }
                else if (!SelectedBookCategories.Any(c => c.CategoryId == value.CategoryId)) SelectedBookCategories.Add(value);

                CategorySearchText = string.Empty; IsCategoryDropdownOpen = false; SelectedCategoryComboItem = null;
            });
        }
        [RelayCommand] private void RemoveCategory(Category category) => SelectedBookCategories.Remove(category);
    }
}