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

        public async Task<IEnumerable<CarInventory>> GetCarsByShowroomIdAsync(int showroomId)
        {
            return await _context.CarInventories
                .Include(inv => inv.Car) // Kéo theo thông tin Xe
                    .ThenInclude(c => c.CarImages) // Từ Xe kéo tiếp bộ Ảnh
                .Where(inv => inv.ShowroomId == showroomId
                           && inv.DisplayStatus != "InWarehouse" // Bỏ qua xe cất trong kho kín
                           && inv.Car != null
                           && inv.Car.IsDeleted == false) // Bỏ qua xe đã bị xóa (Soft Delete)
                .ToListAsync();
        }
    }
}
