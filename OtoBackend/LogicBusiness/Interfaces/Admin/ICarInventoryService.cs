using CoreEntities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface ICarInventoryService
    {
        Task<(bool Success, string Message)> UpdateStockAsync(int carId, int showroomId, int newQuantity, string displayStatus, string? color = null);

        Task<IEnumerable<CarInventory>> GetInventoriesByCarIdAsync(int carId);

        Task<int> GetTotalQuantityAsync(int carId);

        Task<IEnumerable<CarInventory>> GetCarsByShowroomIdAsync(int showroomId);
    }
}