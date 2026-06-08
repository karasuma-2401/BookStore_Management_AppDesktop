using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Helpers
{
    public static class AppSession
    {
        private static string _currentRole = "staff";
        public static string CurrentRole
        {
            get => _currentRole;
            set => _currentRole = value?.ToLowerInvariant() ?? "staff";
        }

        public static bool IsAdmin => string.Equals(CurrentRole, "admin", StringComparison.OrdinalIgnoreCase);
        public static bool IsEmployee => !IsAdmin;
    }
}
