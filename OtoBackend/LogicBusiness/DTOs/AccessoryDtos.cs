using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class AccessoryCreateDto
    {
        [Required(ErrorMessage = "Tên phụ kiện không được để trống!")]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Giá không được âm!")]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class AccessoryUpdateDto
    {
        [Required(ErrorMessage = "Tên phụ kiện không được để trống!")]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Giá không được âm!")]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class AccessoryResponseDto
    {
        public int AccessoryId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }

    // Gán / bỏ gán phụ kiện cho xe
    public class AssignAccessoriesDto
    {
        [Required]
        public List<int> AccessoryIds { get; set; } = new();
    }
}
