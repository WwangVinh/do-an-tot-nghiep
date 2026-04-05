using System.ComponentModel.DataAnnotations;

namespace LogicBusiness.Helpers 
{

    // check năm sản xuất phải nằm trong khoảng từ 1990 đến năm hiện tại
    public class ValidYearAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int year)
            {
                int currentYear = DateTime.Now.Year;

                if (year < 1990 || year > currentYear)
                {
                    return new ValidationResult($"Năm sản xuất phải nằm trong khoảng từ 1990 đến năm {currentYear}!");
                }
            }
            return ValidationResult.Success;
        }
    }
}