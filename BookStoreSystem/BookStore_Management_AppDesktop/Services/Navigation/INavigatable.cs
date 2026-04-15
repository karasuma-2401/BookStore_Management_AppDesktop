using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Services.Navigation
{
    public interface INavigatable
    {
        void OnNavigatedTo(object? parameter);
    }
}
