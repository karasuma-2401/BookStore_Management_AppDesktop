using BookStore_Management_AppDesktop.Helpers;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class ImportViewModel : BaseViewModel
    {
        private readonly IBookApiService _apiService;
        private CancellationTokenSource? _searchCts;
        private readonly DebounceHelper _searchDebouncer = new DebounceHelper();

        // 1. DỮ LIỆU BẢNG TẠP (Draft List) - Nơi chứa các ImportCartItem ta vừa tạo
        [ObservableProperty]
        private ObservableCollection<ImportCartItem> _draftList = new ObservableCollection<ImportCartItem>();

        // 2. NHÓM PHỤC VỤ TÌM KIẾM & GỢI Ý
        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Book> _searchResults = new ObservableCollection<Book>();

        [ObservableProperty]
        private Book? _selectedSearchResult; // Cuốn sách nhân viên click chọn từ thanh gợi ý

        // 3. NHÓM FORM NHẬP NHANH (Trước khi đẩy vào bảng tạm)
        [ObservableProperty]
        private int _tempImportQuantity = 1;

        [ObservableProperty]
        private decimal _tempImportPrice = 0;

        public ImportViewModel(IBookApiService apiService)
        {
            _apiService = apiService;
        }

        // --- LOGIC 1: TÌM KIẾM DEBOUNCE (Tái sử dụng kiến thức từ Task trước) ---
        partial void OnSearchTextChanged(string value)
        {
            _ = _searchDebouncer.RunAsync(400, async (token) =>
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    Application.Current.Dispatcher.Invoke(() => SearchResults.Clear());
                    return;
                }

                var query = new BookQueryParameters { Keyword = SearchText, PageSize = 10 };
                var results = await _apiService.GetAllBooksAsync(query, token);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SearchResults.Clear();
                    foreach (var book in results) SearchResults.Add(book);
                });
            });
        }

        // --- LOGIC 2: THÊM VÀO BẢNG TẠM (Giải quyết Vấn đề 2) ---
        [RelayCommand]
        private void AddToDraft()
        {
            if (SelectedSearchResult == null)
            {
                MessageBox.Show("Vui lòng chọn một cuốn sách từ danh sách gợi ý!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (TempImportQuantity <= 0 || TempImportPrice < 0)
            {
                MessageBox.Show("Số lượng phải > 0 và Giá nhập không được âm!", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // THUẬT TOÁN HƯỚNG B: KIỂM TRA TRÙNG LẶP (Dùng LINQ)
            var existingItem = DraftList.FirstOrDefault(x => x.BookId == SelectedSearchResult.BookId);

            if (existingItem != null)
            {
                // Nếu sách đã có trong bảng tạm -> Tự động cộng dồn
                existingItem.ImportQuantity += TempImportQuantity;

                // Đè giá nhập mới nhất (Theo chuẩn Last-In Pricing chúng ta đã chốt)
                existingItem.ImportPrice = TempImportPrice;
            }
            else
            {
                // Nếu chưa có -> Tạo dòng mới
                var newItem = new ImportCartItem
                {
                    BookId = SelectedSearchResult.BookId,
                    Title = SelectedSearchResult.Title ?? string.Empty,
                    AuthorName = SelectedSearchResult.AuthorName ?? string.Empty,
                    CurrentQuantity = SelectedSearchResult.Quantity,
                    ImportQuantity = TempImportQuantity,
                    ImportPrice = TempImportPrice
                };
                DraftList.Add(newItem);
            }

            // Sau khi thêm xong, dọn dẹp Form để nhân viên nhập cuốn tiếp theo
            SelectedSearchResult = null;
            SearchText = string.Empty;
            TempImportQuantity = 1;
            TempImportPrice = 0;
            SearchResults.Clear();
        }

        // --- LOGIC 3: TẠO SÁCH MỚI (Giải quyết Hướng UX 2) ---
        [RelayCommand]
        private void OpenCreateNewBookDialog()
        {
            // Tạm thời xuất MessageBox để giữ chỗ. 
            // Ở bước sau, chúng ta sẽ gọi UI Dialog (WPF UI ContentDialog) tại đây.
            MessageBox.Show("Tính năng 'Thêm sách mới vào danh mục' (Inline Creation) sẽ được ráp nối UI ở bước sau!", "Thông báo");
        }
    }
}
