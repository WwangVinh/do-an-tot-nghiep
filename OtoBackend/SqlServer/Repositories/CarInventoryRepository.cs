using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class CarInventoryRepository : ICarInventoryRepository
    {
        private readonly OtoContext _context; // Đổi lại tên DbContext nếu hệ thống của ní tên khác

        public CarInventoryRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<CarInventory?> GetInventoryAsync(int carId, int showroomId)
        {
            return await _context.CarInventories
                .FirstOrDefaultAsync(i => i.CarId == carId && i.ShowroomId == showroomId);
        }

        public async Task<IEnumerable<CarInventory>> GetInventoriesByCarIdAsync(int carId)
        {
            // Lấy tất cả các kho đang chứa con xe này, VÀ nối luôn bảng Showroom để lấy tên tỉnh
            return await _context.CarInventories
                .Include(i => i.Showroom) // Bước cực kỳ quan trọng để không bị mù thông tin Showroom
                .Where(i => i.CarId == carId)
                .ToListAsync();
        }

        public async Task AddInventoryAsync(CarInventory inventory)
        {
            await _context.CarInventories.AddAsync(inventory);
            await _context.SaveChangesAsync(); // Chốt lưu DB
        }

        public async Task UpdateInventoryAsync(CarInventory inventory)
        {
            _context.CarInventories.Update(inventory);
            await _context.SaveChangesAsync(); // Chốt lưu DB
        }
        public async Task<int> GetTotalQuantityByCarIdAsync(int carId)
        {
            // Tính tổng số lượng của tất cả các showroom cho cái xe này
            return await _context.CarInventories
                .Where(i => i.CarId == carId)
                .SumAsync(i => i.Quantity);
        }

        public async Task<IEnumerable<CarInventory>> GetCarsByShowroomIdAsync(int showroomId)
        {
            // Nhớ viết thế này ở file CarInventoryRepository.cs nha ní:
            return await _context.CarInventories
         .Include(inv => inv.Car) // Chỉ cần Include mỗi cái này là lấy được hết Hãng, Năm, Giá...
         .Where(inv => inv.ShowroomId == showroomId && inv.Quantity > 0)
         .ToListAsync();
        }
    }
}
