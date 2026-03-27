using CoreEntities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IConsignmentRepository
    {
        // 1. Thêm hồ sơ mới (Khách hàng dùng)
        Task AddAsync(Consignment consignment);

        // 2. Cập nhật hồ sơ (Sếp dùng để chốt giá, đổi trạng thái)
        Task UpdateAsync(Consignment consignment);

        // 3. Lấy 1 hồ sơ cụ thể
        Task<Consignment?> GetByIdAsync(int id);

        // 4. Lấy tất cả hồ sơ cho Sếp xem (Admin/Manager)
        Task<IEnumerable<Consignment>> GetAllAdminAsync();

        // 5. Lấy danh sách hồ sơ của 1 khách hàng cụ thể (Customer)
        Task<IEnumerable<Consignment>> GetByUserIdAsync(int userId);
    }
}