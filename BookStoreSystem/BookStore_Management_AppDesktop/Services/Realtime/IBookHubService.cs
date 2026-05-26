using System;
using System.Threading.Tasks;
using BookStore_Management_AppDesktop.Models;

namespace BookStore_Management_AppDesktop.Services.Realtime
{
    public interface IBookHubService
    {
        event Action<Book>? BookCreated;
        event Action<int>? BookDeleted;
        event Action<int>? BookUpdated;
        event Action<int, int>? InventoryStockChanged;
        event Action? ImportCreated;

        Task StartAsync();
        Task StopAsync();
    }
}