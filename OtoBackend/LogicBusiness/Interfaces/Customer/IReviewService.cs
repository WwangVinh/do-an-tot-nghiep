using CoreEntities.Models;
using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Customer
{
    public interface IReviewService
    {
        // Khách gửi review
        Task<(bool Success, string Message)> SubmitReviewAsync(ReviewSubmitDto dto);

        // Lấy danh sách review đã duyệt của một xe
        Task<List<Review>> GetApprovedReviewsByCarIdAsync(int carId);

        Task<(bool IsEligible, string? FullName, string Message)> CheckReviewEligibilityAsync(string phone, int carId);
    }
}
