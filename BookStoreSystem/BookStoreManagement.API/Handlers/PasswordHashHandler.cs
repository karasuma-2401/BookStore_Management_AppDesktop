using BCrypt.Net;

namespace BookStoreManagement.API.Handlers
{
    public class PasswordHashHandler
    {

        // Dung de bam mat khau khi Dang ky hoac Seed data
        public static string HashPassword(string password)
        {
            // BCrypt tu dong tao Salt nen ban khong can lo phan do
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Dung de kiem tra mat khau khi Dang nhap
        public static bool VerifyPassword(string password, string passwordHash)
        {
            // Tra ve true neu mat khau khop voi chuoi da bam
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

    }
}
