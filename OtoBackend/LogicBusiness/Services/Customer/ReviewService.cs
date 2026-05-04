using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Customer
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IBookingRepository _bookingRepo; // Thêm dòng này

        public ReviewService(
            IReviewRepository reviewRepo,
            IOrderRepository orderRepo,
            IBookingRepository bookingRepo) // Tiêm thêm vào đây
        {
            _reviewRepo = reviewRepo;
            _orderRepo = orderRepo;
            _bookingRepo = bookingRepo; // Gán giá trị
        }

        public async Task<(bool Success, string Message)> SubmitReviewAsync(ReviewSubmitDto dto)
        {
            // 1. Kiểm tra giới hạn 10 lượt/SĐT
            int reviewCount = await _reviewRepo.GetReviewCountByPhoneAsync(dto.Phone);
            if (reviewCount >= 10)
                return (false, "Ní đã hết lượt đánh giá (tối đa 10 lượt/SĐT).");

            // 2. Check var: Có đơn hàng HOẶC lịch hẹn với xe này không?
            var hasOrder = await _orderRepo.HasOrderedCarAsync(dto.Phone, dto.CarId);
            var hasBooking = await _bookingRepo.HasBookedCarAsync(dto.Phone, dto.CarId);

            if (!hasOrder && !hasBooking)
                return (false, "Ní chưa từng đặt cọc hay đặt lịch xem con xe này!");

            // 3. Mapping và Lưu (IsApproved = true để hiện luôn)
            var review = new Review
            {
                CarId = dto.CarId,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.Now,
                IsApproved = true
            };

            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveChangesAsync();

            return (true, "Đánh giá thành công!");
        }

        // Thêm hàm này vào class ReviewService trong file ReviewService.cs
        public async Task<List<Review>> GetApprovedReviewsByCarIdAsync(int carId)
        {
            // Gọi xuống Repository để lấy danh sách review của con xe này
            return await _reviewRepo.GetApprovedReviewsByCarIdAsync(carId);
        }
        public async Task<(bool IsEligible, string? FullName, string Message)> CheckReviewEligibilityAsync(string phone, int carId)
        {
            // 1. Kiểm tra giới hạn 10 lượt/SĐT
            int reviewCount = await _reviewRepo.GetReviewCountByPhoneAsync(phone);
            if (reviewCount >= 10)
                return (false, null, "Ní đã hết lượt đánh giá cho xe này (tối đa 10 lượt).");

            string? customerName = null;

            // 2. Tìm tên khách trong lịch sử Mua xe
            customerName = await _orderRepo.GetCustomerNameFromOrderAsync(phone, carId);

            // 3. Nếu chưa mua, tìm tên khách trong lịch sử Đặt lịch xem xe
            if (string.IsNullOrEmpty(customerName))
            {
                customerName = await _bookingRepo.GetCustomerNameFromBookingAsync(phone, carId);
            }

            // 4. Trả kết quả
            if (string.IsNullOrEmpty(customerName))
            {
                return (false, null, "SĐT này chưa từng mua hay đặt lịch xem xe này. Phải trải nghiệm thật mới được đánh giá nha!");
            }

            return (true, customerName, "Hợp lệ");
        }
    }
}