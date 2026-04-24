using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace JetAdminSystem.Services
{
    public interface ICloudinaryService
    {
        // Hàm upload ảnh mà ông gọi trong BillingsController
        Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folderName);
    }
}