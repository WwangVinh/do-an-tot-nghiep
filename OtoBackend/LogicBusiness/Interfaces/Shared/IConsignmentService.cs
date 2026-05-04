using LogicBusiness.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Shared
{
    public interface IConsignmentService
    {
        // ── KHÁCH HÀNG ────────────────────────────────────────────────
        /// <summary>Khách vãng lai gửi yêu cầu ký gửi xe.</summary>
        Task<(bool Success, string Message)> CreateConsignmentAsync(ConsignmentCreateDto dto);

        // ── ADMIN / MANAGER ───────────────────────────────────────────
        /// <summary>Lấy toàn bộ danh sách hồ sơ ký gửi.</summary>
        Task<IEnumerable<ConsignmentListItemDto>> GetAllAdminAsync();

        /// <summary>Lấy chi tiết 1 hồ sơ ký gửi theo ID.</summary>
        Task<ConsignmentResponseDto?> GetByIdAsync(int consignmentId);

        /// <summary>Cập nhật trạng thái, chốt giá, gắn xe.</summary>
        Task<(bool Success, string Message)> UpdateConsignmentStatusAsync(int consignmentId, ConsignmentUpdateDto dto, string adminRole);
    }
}