using BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.CustomerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public class CustomerViewModel : ObservableObject
    {
        private readonly ICustomerApiService _apiService;
        private readonly IDialogService _dialogService;
        private List<CustomerResponseDto> _allCustomers = new();
        private CancellationTokenSource? _searchCts;

        private ObservableCollection<CustomerResponseDto> _customers = new();
        private string _searchText = string.Empty;
        private string _selectedFilter = "Name";
        private string _searchPlaceholder = "Search by Name...";
        private bool _isLoading = false;
        private int _currentPage = 1;
        private int _pageSize = 8;
        private int _totalPages = 1;
        private decimal _totalDebt = 0;

        public ObservableCollection<CustomerResponseDto> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    OnSearchTextChangedInternal();
                }
            }
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (SetProperty(ref _selectedFilter, value))
                {
                    OnSelectedFilterChangedInternal();
                }
            }
        }

        public string SearchPlaceholder
        {
            get => _searchPlaceholder;
            set => SetProperty(ref _searchPlaceholder, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    OnCurrentPageChangedInternal();
                }
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (SetProperty(ref _pageSize, value))
                {
                    OnPageSizeChangedInternal();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        public decimal TotalDebt
        {
            get => _totalDebt;
            set => SetProperty(ref _totalDebt, value);
        }

        public List<int> PageSizeOptions { get; set; } = new List<int> { 5, 8, 10, 12, 15 };

        public bool IsNameSelected => SelectedFilter == "Name";
        public bool IsPhoneSelected => SelectedFilter == "Phone";
        public bool IsEmailSelected => SelectedFilter == "Email";
        public bool IsDebtSelected => SelectedFilter == "Debt";

        public RelayCommand<string> ChangeFilterCommand { get; private set; }
        public RelayCommand AddCustomerCommand { get; private set; }
        public RelayCommand<CustomerResponseDto> EditCustomerCommand { get; private set; }
        public RelayCommand<CustomerResponseDto> DeleteCustomerCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public CustomerViewModel(ICustomerApiService apiService, IDialogService dialogService)
        {
            _apiService = apiService;
            _dialogService = dialogService;

            ChangeFilterCommand = new RelayCommand<string>(filter =>
            {
                if (filter != null)
                    SelectedFilter = filter;
            });

            AddCustomerCommand = new RelayCommand(async () => await AddCustomerAsync());
            EditCustomerCommand = new RelayCommand<CustomerResponseDto>(async customer => await EditCustomerAsync(customer));
            DeleteCustomerCommand = new RelayCommand<CustomerResponseDto>(async customer => await DeleteCustomerAsync(customer));
            RefreshCommand = new RelayCommand(async () => await RefreshAsync());

            _ = LoadCustomersAsync();
        }

        private async Task LoadCustomersAsync()
        {
            IsLoading = true;
            try
            {
                var data = await _apiService.GetAllCustomersAsync();
                if (data != null)
                {
                    _allCustomers = data;
                    TotalDebt = _allCustomers.Sum(c => c.Debt);
                    UpdateDisplayList();
                }
                else
                {
                    _dialogService.ShowMessage("Failed to load customers from server.");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Error loading customers: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateDisplayList()
        {
            var filteredList = _allCustomers;

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchText))
            {
                filteredList = ApplySearchFilter(filteredList, SearchText, SelectedFilter);
            }

            // Calculate pagination
            TotalPages = (filteredList.Count + PageSize - 1) / PageSize;
            if (CurrentPage > TotalPages && TotalPages > 0)
                CurrentPage = TotalPages;

            // Apply pagination
            var displayList = filteredList
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            Customers.Clear();
            foreach (var customer in displayList)
            {
                Customers.Add(customer);
            }
        }

        private List<CustomerResponseDto> ApplySearchFilter(List<CustomerResponseDto> customers, string searchText, string filterType)
        {
            var searchLower = searchText.ToLower();

            return filterType switch
            {
                "Name" => customers.Where(c => c.Name.ToLower().Contains(searchLower)).ToList(),
                "Phone" => customers.Where(c => (c.Phone ?? "").ToLower().Contains(searchLower)).ToList(),
                "Email" => customers.Where(c => (c.Email ?? "").ToLower().Contains(searchLower)).ToList(),
                "Debt" => customers.Where(c => c.Debt.ToString().Contains(searchText)).ToList(),
                _ => customers
            };
        }

        private void OnSearchTextChangedInternal()
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            Task.Run(async () =>
            {
                await Task.Delay(300, token);
                if (!token.IsCancellationRequested)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        CurrentPage = 1;
                        UpdateDisplayList();
                    });
                }
            }, token);
        }

        private void OnSelectedFilterChangedInternal()
        {
            SearchPlaceholder = SelectedFilter switch
            {
                "Name" => "Search by Name...",
                "Phone" => "Search by Phone...",
                "Email" => "Search by Email...",
                "Debt" => "Search by Debt...",
                _ => "Search..."
            };
            OnPropertyChanged(nameof(IsNameSelected));
            OnPropertyChanged(nameof(IsPhoneSelected));
            OnPropertyChanged(nameof(IsEmailSelected));
            OnPropertyChanged(nameof(IsDebtSelected));
            SearchText = string.Empty;
            CurrentPage = 1;
        }

        private void OnCurrentPageChangedInternal()
        {
            UpdateDisplayList();
        }

        private void OnPageSizeChangedInternal()
        {
            CurrentPage = 1;
            UpdateDisplayList();
        }

        private async Task AddCustomerAsync()
        {
            var window = new Views.Windows.AddCustomerWindow();
            if (window.ShowDialog() == true)
            {
                await LoadCustomersAsync();
            }
        }

        private async Task EditCustomerAsync(CustomerResponseDto? customer)
        {
            if (customer == null)
            {
                _dialogService.ShowMessage("Please select a customer to edit.");
                return;
            }

            var window = new Views.Windows.EditCustomerWindow(customer);
            if (window.ShowDialog() == true)
            {
                await LoadCustomersAsync();
            }
        }

        private async Task DeleteCustomerAsync(CustomerResponseDto? customer)
        {
            if (customer == null)
            {
                _dialogService.ShowMessage("Please select a customer to delete.");
                return;
            }

            if (_dialogService.ShowConfirmation($"Are you sure you want to delete {customer.Name}?"))
            {
                var result = await _apiService.DeleteCustomerAsync(customer.CustomerId);
                if (result)
                {
                    _dialogService.ShowMessage($"Customer {customer.Name} deleted successfully!");
                    await LoadCustomersAsync();
                }
                else
                {
                    _dialogService.ShowMessage("Failed to delete customer. Please try again.");
                }
            }
        }

        private async Task RefreshAsync()
        {
            SearchText = string.Empty;
            SelectedFilter = "Name";
            CurrentPage = 1;
            await LoadCustomersAsync();
        }
    }
}
