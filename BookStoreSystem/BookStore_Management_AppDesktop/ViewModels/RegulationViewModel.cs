using BookStore_Management_AppDesktop.Models.DTOs.RegulationDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Views.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

public partial class RegulationViewModel : ObservableObject
{
    private readonly IRegulationApiService _service;

    [ObservableProperty]
    private ObservableCollection<RegulationResponseDto> regulations = new();

    [ObservableProperty]
    private bool isLoading;

    public RegulationViewModel(IRegulationApiService service)
    {
        _service = service;
    }

    [RelayCommand]
    public async Task LoadData()
    {
        try
        {
            IsLoading = true;

            var data = await _service.GetAllAsync();

            Regulations = data != null
                ? new ObservableCollection<RegulationResponseDto>(data)
                : new ObservableCollection<RegulationResponseDto>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Load error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task Delete(RegulationResponseDto item)
    {
        if (item == null) return;

        if (MessageBox.Show(
            $"Delete '{item.SettingName}'?",
            "Confirm",
            MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            return;

        try
        {
            IsLoading = true;

            await _service.DeleteAsync(item.SettingName);
            await LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Delete error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task Add()
    {
        var window = new RegulationEditView(null)
        {
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        if (window.ShowDialog() == true)
        {
            await LoadData(); 
        }
    }

    [RelayCommand]
    public async Task Edit(RegulationResponseDto item)
    {
        if (item == null) return;

        var window = new RegulationEditView(item)
        {
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        if (window.ShowDialog() == true)
        {
            await LoadData();
        }
    }
}