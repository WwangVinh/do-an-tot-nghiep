using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;

namespace LogicBusiness.Services.Admin
{
    public class ShowroomService : IShowroomService
    {
        private readonly IShowroomRepository _showroomRepo;

        public ShowroomService(IShowroomRepository showroomRepo)
        {
            _showroomRepo = showroomRepo;
        }

        // 1. LẤY TẤT CẢ (Đã bóc tách địa chỉ)
        public async Task<IEnumerable<ShowroomDto>> GetAllShowroomsAsync()
        {
            var showrooms = await _showroomRepo.GetAllAsync();
            return showrooms.Select(s => new ShowroomDto
            {
                ShowroomId = s.ShowroomId,
                Name = s.Name,
                Province = s.Province,
                District = s.District,
                StreetAddress = s.StreetAddress,
                FullAddress = s.FullAddress, // Xài cái getter xịn mình mới thêm ở Model
                Hotline = s.Hotline
            });
        }

        // 2. LẤY CHI TIẾT THEO ID
        public async Task<ShowroomDto?> GetShowroomByIdAsync(int id)
        {
            var s = await _showroomRepo.GetByIdAsync(id);
            if (s == null) return null;

            return new ShowroomDto
            {
                ShowroomId = s.ShowroomId,
                Name = s.Name,
                Province = s.Province,
                District = s.District,
                StreetAddress = s.StreetAddress,
                FullAddress = s.FullAddress,
                Hotline = s.Hotline
            };
        }

        // 3. TẠO MỚI (Bắt Admin nhập chuẩn 3 thành phần)
        public async Task<(bool Success, string Message)> CreateShowroomAsync(ShowroomCreateDto dto)
        {
            // Kiểm tra trùng lặp dựa trên combo: Tên + Tỉnh + Huyện + Đường
            if (await _showroomRepo.CheckExistsAsync(dto.Name.Trim(), dto.Province.Trim(), dto.District.Trim(), dto.StreetAddress.Trim()))
            {
                return (false, "Cơ sở này đã tồn tại ở địa chỉ này rồi ní ơi!");
            }

            var showroom = new Showroom
            {
                Name = dto.Name.Trim(),
                Province = dto.Province.Trim(),
                District = dto.District.Trim(),
                StreetAddress = dto.StreetAddress.Trim(),
                Hotline = dto.Hotline?.Trim()
            };

            await _showroomRepo.AddAsync(showroom);
            return (true, "Thêm cơ sở mới cực kỳ chuẩn chỉ!");
        }

        // 4. CẬP NHẬT
        public async Task<(bool Success, string Message)> UpdateShowroomAsync(int id, ShowroomUpdateDto dto)
        {
            var showroom = await _showroomRepo.GetByIdAsync(id);
            if (showroom == null) return (false, "Không tìm thấy cơ sở!");

            // Kiểm tra trùng lặp (trừ chính nó ra)
            if (await _showroomRepo.CheckExistsAsync(dto.Name.Trim(), dto.Province.Trim(), dto.District.Trim(), dto.StreetAddress.Trim(), id))
            {
                return (false, "Thông tin này bị trùng với một chi nhánh khác mất rồi!");
            }

            showroom.Name = dto.Name.Trim();
            showroom.Province = dto.Province.Trim();
            showroom.District = dto.District.Trim();
            showroom.StreetAddress = dto.StreetAddress.Trim();
            showroom.Hotline = dto.Hotline?.Trim();

            await _showroomRepo.UpdateAsync(showroom);
            return (true, "Cập nhật địa chỉ showroom thành công!");
        }

        // 5. XÓA (Giữ nguyên logic cũ)
        public async Task<(bool Success, string Message)> DeleteShowroomAsync(int id)
        {
            var showroom = await _showroomRepo.GetByIdAsync(id);
            if (showroom == null) return (false, "Không tìm thấy cơ sở!");

            await _showroomRepo.DeleteAsync(showroom);
            return (true, "Xóa cơ sở thành công!");
        }
    }
}