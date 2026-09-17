using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Infrastructure.Services
{
    public class CloudinaryService : IUploadCloud
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new InvalidOperationException("Cấu hình Cloudinary không hợp lệ.");
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(byte[] fileBytes, string fileName, string folder, CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream(fileBytes);
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (extension == ".html" || extension == ".htm")
            {
                var rawUploadParams = new RawUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Folder = folder
                };
                var rawResult = await Task.Run(() => _cloudinary.Upload(rawUploadParams), cancellationToken);
                if (rawResult.Error != null)
                {
                    throw new Exception(rawResult.Error.Message);
                }
                return rawResult.SecureUrl.ToString();
            }

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = folder
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            return uploadResult.SecureUrl.ToString();
        }

        public async Task<string> UploadVideoAsync(byte[] fileBytes, string fileName, string folder, CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream(fileBytes);
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = folder
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            return uploadResult.SecureUrl.ToString();
        }
    }
}
