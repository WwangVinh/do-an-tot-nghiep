using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreEntities.Models;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface ICarInventoryRepository
    {
        // Hàm tìm xem xe này ở showroom này đã có trong kho chưa
        Task<CarInventory?> GetInventoryAsync(int carId, int showroomId);
        Task<IEnumerable<CarInventory>> GetInventoriesByCarIdAsync(int carId);

        // Hàm thêm xe mới vào kho
        Task AddInventoryAsync(CarInventory inventory);

        // Hàm cập nhật số lượng xe trong kho
        Task UpdateInventoryAsync(CarInventory inventory);
        Task<int> GetTotalQuantityByCarIdAsync(int carId);
    }
}
