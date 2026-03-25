using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Customer
{
    public class CarWishlistService : ICarWishlistService
    {
        private readonly ICarWishlistRepository _wishlistRepo;
        private readonly ICarRepository _carRepo;

        public CarWishlistService(ICarWishlistRepository wishlistRepo, ICarRepository carRepo)
        {
            _wishlistRepo = wishlistRepo;
            _carRepo = carRepo;
        }

        // CÁI NÀY LÀ LINH HỒN CỦA TÍNH NĂNG NÈ:
        public async Task<(bool Success, string Message, bool IsHearted)> ToggleWishlistAsync(int userId, int carId)
        {
            // Kiểm tra xe có tồn tại không đã
            var car = await _carRepo.GetByIdAsync(carId);
            if (car == null) return (false, "Không tìm thấy xe này!", false);

            var existingItem = await _wishlistRepo.GetByUserAndCarAsync(userId, carId);

            if (existingItem != null)
            {
                // Đã thả tim rồi -> Giờ bấm nữa là BỎ TIM (Xóa)
                await _wishlistRepo.DeleteAsync(existingItem);
                return (true, "Đã bỏ lưu xe khỏi danh sách yêu thích.", false); // Trả về false để FE đổi icon tim rỗng
            }
            else
            {
                // Chưa thả tim -> Giờ là THẢ TIM (Thêm mới)
                var newWishlist = new CarWishlist { UserId = userId, CarId = carId };
                await _wishlistRepo.AddAsync(newWishlist);
                return (true, "Đã lưu xe vào danh sách yêu thích! ❤️", true); // Trả về true để FE đổi icon tim đỏ
            }
        }

        public async Task<IEnumerable<WishlistResponseDto>> GetMyWishlistAsync(int userId)
        {
            var data = await _wishlistRepo.GetMyWishlistAsync(userId);

            return data.Select(w => new WishlistResponseDto
            {
                WishlistId = w.WishlistId,
                CarId = (int)w.CarId,
                CarName = w.Car?.Name ?? "Xe không xác định",
                Brand = w.Car?.Brand ?? "",
                Year = w.Car?.Year ?? 0,
                Price = w.Car?.Price ?? 0,
                ImageUrl = w.Car?.ImageUrl,
                Status = w.Car?.Status.ToString() ?? ""
            });
        }
    }
}