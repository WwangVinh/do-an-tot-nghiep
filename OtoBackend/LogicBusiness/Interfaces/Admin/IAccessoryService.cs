using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface IAccessoryService
    {
        // CRUD phụ kiện
        Task<IEnumerable<AccessoryResponseDto>> GetAllAsync();
        Task<AccessoryResponseDto?> GetByIdAsync(int id);
        Task<(bool Success, string Message, AccessoryResponseDto? Data)> CreateAsync(AccessoryCreateDto dto);
        Task<(bool Success, string Message)> UpdateAsync(int id, AccessoryUpdateDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int id);

        // Gán phụ kiện cho xe
        Task<IEnumerable<AccessoryResponseDto>> GetByCarIdAsync(int carId);
        Task<(bool Success, string Message)> AssignToCarAsync(int carId, AssignAccessoriesDto dto);
        Task<(bool Success, string Message)> RemoveFromCarAsync(int carId, AssignAccessoriesDto dto);
    }
}
