using BookStore_Management_AppDesktop.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookStore_Management_AppDesktop.ViewModels.Base
{
    public abstract class BaseViewModel : ObservableObject
    {
        protected BaseViewModel()
        {
            WeakReferenceMessenger.Default.Register<RefreshDataMessage>(this, async (recipient, message) =>
            {
                await LoadDataAsync();
            });
        }
        public virtual async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }
    }
}
