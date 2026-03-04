using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LogicBusiness.DTOs
{
    public class FeatureUpdateDto
    {
        public string FeatureName { get; set; } = null!;

        // Ảnh mới (nếu có up thì thay, không up thì để trống hệ thống sẽ giữ ảnh cũ)
        public IFormFile? Icon { get; set; }
    }
}