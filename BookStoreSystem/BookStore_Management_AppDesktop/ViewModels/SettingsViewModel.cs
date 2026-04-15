using BookStore_Management_AppDesktop.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        public Action<string>? OnShowMessage { get; set; }

        [RelayCommand]
        private void RefreshData()
        {
            WeakReferenceMessenger.Default.Send(new RefreshDataMessage());

            OnShowMessage?.Invoke("Data has been successfully refreshed from the server!");
        }
    }
}
