using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.ImportDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.Import;
using BookStore_Management_AppDesktop.Services.Realtime;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class ImportCreateViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IImportApiService _importApiService;
        private readonly IBookHubService _hubService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty] private ObservableCollection<ImportCartItem> _draftList = new ObservableCollection<ImportCartItem>();
        [ObservableProperty] private decimal _totalDraftAmount = 0;

        public ImportCreateViewModel(
            IDialogService dialogService,
            IImportApiService importApiService,
            IBookHubService hubService,
            IServiceProvider serviceProvider)
        {
            _dialogService = dialogService;
            _importApiService = importApiService;
            _hubService = hubService;
            _serviceProvider = serviceProvider;

            _hubService.BookCreated += OnBookCreatedRealtime;
        }

        [RelayCommand]
        private void OpenSelectBookDialog()
        {
            var selectBookVM = _serviceProvider.GetRequiredService<SelectBookViewModel>();

            selectBookVM.InitializeCallback(AddItemToDraft);

            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = new BookStore_Management_AppDesktop.Views.Windows.SelectBookWindow(selectBookVM);
                window.ShowDialog();
            });
        }

        private void AddItemToDraft(Book selectedBook, int quantity, decimal importPrice)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var existingItem = DraftList.FirstOrDefault(x => x.BookId == selectedBook.BookId);

                if (existingItem != null)
                {
                    existingItem.ImportQuantity += quantity;
                    existingItem.ImportPrice = importPrice; 
                    RecalculateTotalAmount();
                }
                else
                {
                    var newItem = new ImportCartItem
                    {
                        BookId = selectedBook.BookId,
                        Title = selectedBook.Title ?? string.Empty,
                        AuthorName = selectedBook.DisplayAuthorNames,
                        CurrentQuantity = selectedBook.Quantity,
                        ImportQuantity = quantity,
                        ImportPrice = importPrice
                    };
                    newItem.PropertyChanged += DraftItem_PropertyChanged;
                    DraftList.Add(newItem);
                    RecalculateTotalAmount();
                }
            });
        }
        private void RecalculateTotalAmount() => TotalDraftAmount = DraftList.Sum(item => item.TotalLinePrice);

        private void DraftItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImportCartItem.ImportQuantity) || e.PropertyName == nameof(ImportCartItem.ImportPrice))
            {
                RecalculateTotalAmount();
            }
        }

        [RelayCommand]
        private void RemoveFromDraft(ImportCartItem itemToRemove)
        {
            if (itemToRemove != null)
            {
                itemToRemove.PropertyChanged -= DraftItem_PropertyChanged;
                DraftList.Remove(itemToRemove);
                RecalculateTotalAmount();
            }
        }

        [RelayCommand]
        private void ClearDraft()
        {
            bool isConfirmed = _dialogService.ShowConfirmation("Are you sure you want to clear the entire draft?", "Clear Draft", true);
            if (isConfirmed) ClearDraftSafe();
        }

        public void ClearDraftSafe()
        {
            foreach (var item in DraftList) item.PropertyChanged -= DraftItem_PropertyChanged;
            DraftList.Clear();
            RecalculateTotalAmount();
        }

        [RelayCommand]
        private async Task ConfirmImport()
        {
            if (!DraftList.Any())
            {
                _dialogService.ShowMessage("Your draft list is empty!");
                return;
            }

            bool isConfirmed = _dialogService.ShowConfirmation("Do you want to process and confirm this import order?", "Process Import", false);

            if (isConfirmed)
            {
                var importDto = new ImportCreateDTO
                {
                    Details = DraftList.Select(item => new ImportDetailCreateDTO
                    {
                        BookId = item.BookId,
                        Quantity = item.ImportQuantity,
                        ImportPrice = item.ImportPrice
                    }).ToList()
                };

                try
                {
                    bool isSuccess = await _importApiService.CreateImportAsync(importDto);
                    if (isSuccess)
                    {
                        _dialogService.ShowMessage("Import successful! The inventory has been updated.");
                        ClearDraftSafe();
                    }
                    else
                    {
                        _dialogService.ShowMessage("Import failed. Please check the server connection.");
                    }
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessage($"An error occurred: {ex.Message}");
                }
            }
        }

        private void OnBookCreatedRealtime(Book newBook)
        {
        }
    }
}