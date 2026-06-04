using Microsoft.AspNetCore.SignalR;
using BookStoreManagement.API.Models.Book;

namespace BookStoreManagement.API.Hubs
{
    public interface IBookHubClient
    {
        Task BookCreated(BookResponseDto book);
        Task BookUpdated(int bookId, object updatedFields);
        Task BookDeleted(int bookId);
        Task InventoryStockChanged(int bookId, int newQuantity);
        Task ImportCreated();

        Task AuthorUpdated(int authorId, string newName);
    }

    public class BookHub : Hub<IBookHubClient>
    {

    }
}