using CoreEntities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IConsignmentRepository
    {
        // Thêm hồ sơ mới (Khách hàng dùng)
        Task AddAsync(Consignment consignment);

        // Cập nhật hồ sơ (Sếp dùng để chốt giá, đổi trạng thái)
        Task UpdateAsync(Consignment consignment);

        // Lấy 1 hồ sơ cụ thể
        Task<Consignment?> GetByIdAsync(int id);

        // Lấy tất cả hồ sơ cho Sếp xem (Admin/Manager)
        Task<IEnumerable<Consignment>> GetAllAdminAsync();

        // Lấy danh sách hồ sơ của 1 khách hàng cụ thể (Customer)
        Task<IEnumerable<Consignment>> GetByUserIdAsync(int userId);
    }
}