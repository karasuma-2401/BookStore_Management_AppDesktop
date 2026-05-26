using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.ViewModels.Base
{
    public abstract class BaseViewModel : ObservableObject
    {
        protected BaseViewModel()
        {
        }

        public virtual async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }
    }
}
