using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace LogicBusiness.Services.Admin
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        // Tiêm IWebHostEnvironment để Backend biết thư mục gốc (wwwroot) đang nằm ở đâu trên ổ cứng
        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ hoặc trống!");

            // 1. Chỉ định đường dẫn lưu: wwwroot/uploads/{folderName}
            string uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", folderName);

            // Nếu thư mục chưa tồn tại thì tự động tạo mới
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 2. Tạo tên file độc nhất (Tránh việc up 2 ảnh cùng tên bị ghi đè)
            // Sinh ra 1 đoạn mã ngẫu nhiên (GUID) ghép với tên file gốc
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 3. Copy file từ luồng mạng (Network stream) thả vào ổ cứng
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 4. Trả về đường dẫn tương đối để lưu vào Cột ImageUrl trong SQL
            return $"/uploads/{folderName}/{uniqueFileName}";
        }

        public void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            // Chuyển URL thành đường dẫn vật lý trên ổ cứng
            var filePath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), imageUrl.TrimStart('/'));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
