using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreEntities.Models;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface ICarInventoryRepository
    {
        Task<CarInventory?> GetInventoryAsync(int carId, int showroomId, string? color = null);
        Task<IEnumerable<CarInventory>> GetInventoriesByCarIdAsync(int carId);
        Task AddInventoryAsync(CarInventory inventory);
        Task UpdateInventoryAsync(CarInventory inventory);
        Task<int> GetTotalQuantityByCarIdAsync(int carId);
        Task<IEnumerable<CarInventory>> GetCarsByShowroomIdAsync(int showroomId);
        Task<bool> DeleteInventoriesByCarIdAsync(int carId);
    }
}