using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class CarWishlistRepository : ICarWishlistRepository
    {
        private readonly OtoContext _context;
        public CarWishlistRepository(OtoContext context) => _context = context;

        // Tìm xem khách đã thả tim con xe này chưa
        public async Task<CarWishlist?> GetByUserAndCarAsync(int userId, int carId)
        {
            return await _context.CarWishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.CarId == carId);
        }

        // Lấy danh sách xe đã thả tim (Nhớ Include bảng Car để lấy hình, giá, tên...)
        public async Task<IEnumerable<CarWishlist>> GetMyWishlistAsync(int userId)
        {
            return await _context.CarWishlists
                .Include(w => w.Car)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.WishlistId)
                .ToListAsync();
        }

        public async Task AddAsync(CarWishlist wishlist)
        {
            await _context.CarWishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CarWishlist wishlist)
        {
            _context.CarWishlists.Remove(wishlist);
            await _context.SaveChangesAsync();
        }
    }
}