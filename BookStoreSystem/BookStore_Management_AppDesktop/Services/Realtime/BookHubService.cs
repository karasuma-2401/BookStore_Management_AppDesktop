using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.ImportDTOs;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.Realtime
{
    public class BookHubService : IBookHubService
    {
        private readonly HubConnection _connection;

        public event Action<Book>? BookCreated;
        public event Action<int>? BookDeleted;
        public event Action<int>? BookUpdated;
        public event Action<int, int>? InventoryStockChanged;
        public event Action? ImportCreated;

        public BookHubService()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7063/hub/books", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(Settings.Default.AccessToken ?? string.Empty);
                })
                .WithAutomaticReconnect() 
                .Build();

            _connection.On<Book>("BookCreated", (book) => BookCreated?.Invoke(book));

            _connection.On<int>("BookDeleted", (bookId) => BookDeleted?.Invoke(bookId));

            _connection.On<int, System.Text.Json.JsonElement>("BookUpdated", (bookId, updatedFields) =>
                BookUpdated?.Invoke(bookId));

            _connection.On<int, int>("InventoryStockChanged", (bookId, newQuantity) =>
                InventoryStockChanged?.Invoke(bookId, newQuantity));

            _connection.On("ImportCreated", () => ImportCreated?.Invoke());
            _connection.Reconnecting += (error) =>
            {
                Debug.WriteLine("[SignalR] Connection lost. Reconnecting...");
                return Task.CompletedTask;
            };

            _connection.Reconnected += (connectionId) =>
            {
                Debug.WriteLine("[SignalR] Reconnected successfully.");
                return Task.CompletedTask;
            };
        }

        public async Task StartAsync()
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _connection.StartAsync();
                    Debug.WriteLine("[SignalR] Hub connection started successfully.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SignalR Error] Failed to start connection: {ex.Message}");
                }
            }
        }

        public async Task StopAsync()
        {
            if (_connection.State != HubConnectionState.Disconnected)
            {
                await _connection.StopAsync();
            }
        }
    }
}