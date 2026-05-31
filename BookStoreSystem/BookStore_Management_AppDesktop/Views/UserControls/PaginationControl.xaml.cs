using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookStore_Management_AppDesktop.Views.UserControls
{
    /// <summary>
    /// Interaction logic for PaginationControl.xaml
    /// Component phân trang dùng chung toàn hệ thống chuẩn SOLID.
    /// </summary>
    public partial class PaginationControl : UserControl
    {
        public PaginationControl()
        {
            InitializeComponent();
        }

        //  Thuộc tính Trang hiện tại 
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(int), typeof(PaginationControl), new PropertyMetadata(1));

        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        // 2. Thuộc tính Tổng số trang 
        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register(nameof(TotalPages), typeof(int), typeof(PaginationControl), new PropertyMetadata(1));

        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        // 3. Thuộc tính Tổng số thực thể dữ liệu 
        public static readonly DependencyProperty TotalItemsProperty =
            DependencyProperty.Register(nameof(TotalItems), typeof(int), typeof(PaginationControl), new PropertyMetadata(0));

        public int TotalItems
        {
            get => (int)GetValue(TotalItemsProperty);
            set => SetValue(TotalItemsProperty, value);
        }

        // Thuộc tính Tên hiển thị của thực thể (ví dụ: books, items, invoices,...) 
        public static readonly DependencyProperty ItemNameProperty =
            DependencyProperty.Register(nameof(ItemName), typeof(string), typeof(PaginationControl), new PropertyMetadata("items"));

        public string ItemName
        {
            get => (string)GetValue(ItemNameProperty);
            set => SetValue(ItemNameProperty, value);
        }

        //  Lệnh điều hướng lùi trang
        public static readonly DependencyProperty PreviousPageCommandProperty =
            DependencyProperty.Register(nameof(PreviousPageCommand), typeof(ICommand), typeof(PaginationControl), new PropertyMetadata(null));

        public ICommand PreviousPageCommand
        {
            get => (ICommand)GetValue(PreviousPageCommandProperty);
            set => SetValue(PreviousPageCommandProperty, value);
        }

        // Lệnh điều hướng tiến trang 
        public static readonly DependencyProperty NextPageCommandProperty =
            DependencyProperty.Register(nameof(NextPageCommand), typeof(ICommand), typeof(PaginationControl), new PropertyMetadata(null));

        public ICommand NextPageCommand
        {
            get => (ICommand)GetValue(NextPageCommandProperty);
            set => SetValue(NextPageCommandProperty, value);
        }
    }
}