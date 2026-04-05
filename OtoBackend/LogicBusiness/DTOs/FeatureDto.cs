using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class FeatureCreateDto
    {
        public string FeatureName { get; set; } = null!;
        public IFormFile? Icon { get; set; }
    }

    public class FeatureUpdateDto
    {
        public string FeatureName { get; set; } = null!;
        public IFormFile? Icon { get; set; }
    }
}