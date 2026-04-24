using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using JetAdminSystem.Models;
using Microsoft.Extensions.Options;
using Account = CloudinaryDotNet.Account;

namespace JetAdminSystem.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folderName)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folderName
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }
    }
}