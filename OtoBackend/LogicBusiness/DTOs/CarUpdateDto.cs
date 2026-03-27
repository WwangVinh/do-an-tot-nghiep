using CoreEntities.Models;
using Microsoft.AspNetCore.Http;
using OtoBackend.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class CarUpdateDto
    {
        [Required(ErrorMessage = "Tên xe không được để trống!")]
        [StringLength(255, ErrorMessage = "Tên xe quá dài, tối đa 255 ký tự thôi!")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên hãng xe!")]
        public string Brand { get; set; } = null!;
        public string? Model { get; set; }
        public string? Color { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá xe phải lớn hơn hoặc bằng 0!")]
        public decimal Price { get; set; }

        [RegularExpression("^(Xăng|Điện|Dầu|Hybrid)$", ErrorMessage = "Nhiên liệu chỉ được nhập Xăng, Điện, Dầu hoặc Hybrid!")]
        public string? FuelType { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Số Km (ODO) không được âm!")]
        public double? Mileage { get; set; }
        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng xe không được nhỏ hơn 0!")]

        public int ShowroomId { get; set; }
        public int Quantity { get; set; }

        [RegularExpression("^(Số sàn|Số tự động)$", ErrorMessage = "Hộp số chỉ được nhập 'Số sàn' hoặc 'Số tự động'")]
        public string? Transmission { get; set; }


        [RegularExpression("^(Sedan|SUV|Hatchback|Crossover|MPV|Bán tải|Coupe)$", ErrorMessage = "Kiểu dáng xe không hợp lệ. Vui lòng chọn đúng danh mục!")]
        public string? BodyStyle { get; set; }
        [ValidYearAttribute] // 👈 Tự động kiểm tra từ 1990 đến năm hiện tại!
        public int Year { get; set; }
        public int Condition { get; set; }

        // Trường này để nhận File ảnh mới (nếu có)
        public IFormFile? ImageFile { get; set; }

        // Danh sách FeatureIds (VD: "1,2,3")
        public string? FeatureIds { get; set; }

        // Chuỗi Specifications thần thánh của tụi mình nè ní!
        // Định dạng: "Nhóm|Tên|Giá trị ; Nhóm|Tên|Giá trị"
        public string? Specifications { get; set; }

        public CarStatus? Status { get; set; }
    }
}