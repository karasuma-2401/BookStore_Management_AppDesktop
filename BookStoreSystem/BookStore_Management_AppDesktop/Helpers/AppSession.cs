using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Helpers
{
    public static class AppSession
    {
        public static string CurrentRole { get; set; } = "staff"; 
        public static bool IsAdmin => CurrentRole == "admin";
    }
}
