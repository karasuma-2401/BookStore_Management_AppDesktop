using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]);

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> UploadImageAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(filePath),
                Folder = "BookStore_Images"
            };

            for (int i = 0; i < 2; i++)
            {
                try
                {
                    var result = await _cloudinary.UploadAsync(uploadParams);

                    if (result.StatusCode == System.Net.HttpStatusCode.OK && result.SecureUrl != null)
                        return result.SecureUrl.ToString();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"try upload {i + 1} th fail !!!: {ex.Message}");
                }

                await Task.Delay(500); 
            }

            throw new Exception("Upload failed after retry due to network or server issues.");
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return true;

            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) ||
                !uri.Host.Contains("cloudinary.com"))
                return true;

            try
            {
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                int uploadIndex = Array.IndexOf(segments, "upload");
                if (uploadIndex == -1) return false;

                int start = uploadIndex + 1;

                if (start < segments.Length && segments[start].StartsWith("v"))
                    start++;

                if (start >= segments.Length) return false;

                var publicIdWithExt = string.Join("/", segments.Skip(start));
                var publicId = Path.ChangeExtension(publicIdWithExt, null);

                var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));

                return result.Result == "ok";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error Delete Picture Cloudinary: {ex}");
                return false;
            }
        }
    }
}