using System.ComponentModel.DataAnnotations;

namespace OtoBackend.Helpers // Đổi namespace cho khớp project của ní nha
{
    public class ValidYearAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int year)
            {
                int currentYear = DateTime.Now.Year; // Lấy đúng năm hiện tại của hệ thống

                if (year < 1990 || year > currentYear)
                {
                    return new ValidationResult($"Năm sản xuất phải nằm trong khoảng từ 1990 đến năm {currentYear}!");
                }
            }
            return ValidationResult.Success; // Hợp lệ thì cho qua
        }
    }
}