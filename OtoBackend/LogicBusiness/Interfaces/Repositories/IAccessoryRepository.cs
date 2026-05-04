using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IAccessoryRepository
    {
        Task<IEnumerable<Accessory>> GetAllAsync();
        Task<Accessory?> GetByIdAsync(int id);
        Task AddAsync(Accessory accessory);
        Task UpdateAsync(Accessory accessory);
        Task<bool> DeleteAsync(int id);

        // CarAccessory
        Task<IEnumerable<Accessory>> GetByCarIdAsync(int carId);
        Task AssignToCarAsync(int carId, List<int> accessoryIds);
        Task RemoveFromCarAsync(int carId, List<int> accessoryIds);
        Task<bool> IsAssignedToCarAsync(int carId, int accessoryId);
    }
}
