using BookStore_Management_AppDesktop.Models.DTOs.RegulationDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class RegulationEditViewModel : ObservableObject
    {
        private readonly IRegulationApiService _service;
        private readonly RegulationResponseDto? _originalItem;

        public Action<bool?>? CloseAction;

        [ObservableProperty]
        private string settingName = string.Empty;

        [ObservableProperty]
        private string value = string.Empty;

        [ObservableProperty]
        private bool isSaving;

        public bool IsEditMode { get; }
        public string WindowTitle => IsEditMode ? "Edit Regulation" : "Add Regulation";

        public RegulationEditViewModel(IRegulationApiService service, RegulationResponseDto? item)
        {
            _service = service;
            _originalItem = item;
            IsEditMode = item != null;

            if (item != null)
            {
                SettingName = item.SettingName;
                Value = item.Value;
            }
        }

        [RelayCommand]
        public async Task Save()
        {
            if (string.IsNullOrWhiteSpace(SettingName))
            {
                CustomMessageBox.Show("Setting Name is required");
                return;
            }

            if (string.IsNullOrWhiteSpace(Value))
            {CustomMessageBox.Show("Value is required");
                return;
            }

            try
            {
                IsSaving = true;

                if (IsEditMode)
                {
                    await _service.UpdateAsync(
                        _originalItem!.SettingName,
                        new RegulationUpdateDto { Value = Value });
                }
                else
                {
                    await _service.CreateAsync(
                        new RegulationCreateDto
                        {
                            SettingName = SettingName,
                            Value = Value
                        });
                }

                CustomMessageBox.Show("Saved successfully");

                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }
    }
}