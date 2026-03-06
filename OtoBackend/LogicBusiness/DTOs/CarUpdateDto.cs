using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LogicBusiness.DTOs
{
    public class CarUpdateDto
    {
        [Required(ErrorMessage = "Tên xe không được để trống")]
        public string Name { get; set; } = null!;

        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Color { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public double Price { get; set; }

        public string? FuelType { get; set; }
        public double Mileage { get; set; }
        public string? Description { get; set; }

        public int Year { get; set; }
        public int Condition { get; set; }

        // Trường này để nhận File ảnh mới (nếu có)
        public IFormFile? ImageFile { get; set; }

        // Danh sách FeatureIds (VD: "1,2,3")
        public string? FeatureIds { get; set; }

        // Chuỗi Specifications thần thánh của tụi mình nè ní!
        // Định dạng: "Nhóm|Tên|Giá trị ; Nhóm|Tên|Giá trị"
        public string? Specifications { get; set; }
    }
}