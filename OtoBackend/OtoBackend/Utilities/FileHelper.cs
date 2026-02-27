using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OtoBackend.Utilities
{
    public static class FileHelper
    {
        /// <summary>
        /// Hàm dùng chung để lưu ảnh vào các thư mục phân cấp
        /// </summary>
        /// <param name="file">File ảnh từ ông Shipper</param>
        /// <param name="subFolder">Thư mục chính (Cars, Users, Banners...)</param>
        /// <param name="targetName">Tên định danh (Tên xe hoặc UserId) để gom nhóm ảnh</param>
        /// <returns>Đường dẫn link ảnh để lưu vào Database</returns>
        public static async Task<string> UploadFileAsync(IFormFile file, string subFolder, string targetName)
        {
            if (file == null || file.Length == 0) return null;

            // 1. Chuẩn hóa tên thư mục (Xóa khoảng trắng, ký tự đặc biệt)
            string cleanTargetName = string.Join("_", targetName.Split(Path.GetInvalidFileNameChars()));
            cleanTargetName = cleanTargetName.Replace(" ", "_");

            // 2. Xây dựng đường dẫn: wwwroot/uploads/Cars/VinFast_VF7
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", subFolder, cleanTargetName);

            // 3. Tự động tạo cây thư mục nếu chưa có
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 4. Tạo tên file ngẫu nhiên để không trùng
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            // 5. Lưu file vào ổ cứng
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 6. Trả về link tương đối: /uploads/Cars/VinFast_VF7/abc.jpg
            return $"/uploads/{subFolder}/{cleanTargetName}/{fileName}";
        }
    }
}