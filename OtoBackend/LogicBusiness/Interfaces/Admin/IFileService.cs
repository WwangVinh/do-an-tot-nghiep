using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface IFileService
    {
        // Nhận vào 1 file từ FE, và tên thư mục muốn lưu (vd: "banners" hoặc "cars")
        Task<string> UploadImageAsync(IFormFile file, string folderName);
        void DeleteImage(string imageUrl);
    }
}
