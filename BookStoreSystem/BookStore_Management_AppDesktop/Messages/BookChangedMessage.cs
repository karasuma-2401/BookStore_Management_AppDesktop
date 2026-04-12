using BookStore_Management_AppDesktop.Models; 

namespace BookStore_Management_AppDesktop.Messages
{
    public class BookChangedMessage
    {
        public enum ChangeAction { Add, Update, Delete}

        public ChangeAction Action { get; }
        public Book ChangedBook { get; }

        public BookChangedMessage(ChangeAction action, Book changedBook)
        {
            Action = action;
            ChangedBook = changedBook;
        }
    }
}
