using CoreEntities.Models;
using SqlServer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicBusiness.Interfaces.Repositories;

namespace SqlServer.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly OtoContext _context;

        public ReviewRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public async Task<List<Review>> GetApprovedReviewsByCarIdAsync(int carId)
        {
            return await _context.Reviews
                .Where(r => r.CarId == carId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Thêm vào ReviewRepository.cs
        public async Task<bool> IsAlreadyReviewedAsync(string orderCode)
        {
            // Kiểm tra xem đã có bản ghi nào dùng mã đơn hàng này chưa
            return await _context.Reviews.AnyAsync(r => r.OrderCode == orderCode);
        }

        public async Task<int> GetReviewCountByPhoneAsync(string phone)
        {
            return await _context.Reviews.CountAsync(r => r.Phone == phone);
        }
    }
}
