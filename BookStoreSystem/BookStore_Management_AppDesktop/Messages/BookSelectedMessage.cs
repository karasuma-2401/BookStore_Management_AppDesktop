using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Messages
{
    namespace BookStore_Management_AppDesktop.Messages
    {
        public class BookSelectedMessage : ValueChangedMessage<int>
        {
            public BookSelectedMessage(int bookId) : base(bookId)
            {
            }
        }
    }
}
