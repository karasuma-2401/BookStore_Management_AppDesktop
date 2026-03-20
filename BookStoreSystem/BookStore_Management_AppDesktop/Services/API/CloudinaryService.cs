using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
       
            Account account = new Account(
                "dktghvlmb",
                "177125943557715",
                "9G8FwD7rd0J4OZbUbgSEif7CVkM");

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> UploadImageAsync(string filePath)
        {
            try
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(filePath),
                    Folder = "BookStore_Images" 
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return uploadResult.SecureUrl.ToString(); 
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi upload ảnh: {ex.Message}");
                return string.Empty;
            }
        }

        // Thêm hàm này vào dưới hàm UploadImageAsync
        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            // Nếu link rỗng hoặc không phải link Cloudinary thì bỏ qua
            if (string.IsNullOrEmpty(imageUrl) || !imageUrl.Contains("cloudinary.com"))
                return true;

            try
            {
                // 1. Thuật toán cắt chuỗi URL để lấy PublicId
                var parts = imageUrl.Split('/');

                // Lấy tên thư mục (ví dụ: BookStore_Images)
                string folder = parts[parts.Length - 2];

                // Lấy tên file có đuôi (ví dụ: anh-bia-123.jpg)
                string fileWithExtension = parts[parts.Length - 1];

                // Bỏ đuôi .jpg / .png đi
                string fileName = fileWithExtension.Split('.')[0];

                // Ghép lại thành PublicId chuẩn
                string publicId = $"{folder}/{fileName}";

                // 2. Gửi lệnh Destroy (Tiêu hủy) lên Cloudinary
                var destroyParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(destroyParams);

                return result.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi xóa ảnh trên Cloudinary: {ex.Message}");
                return false;
            }
        }
    }
}