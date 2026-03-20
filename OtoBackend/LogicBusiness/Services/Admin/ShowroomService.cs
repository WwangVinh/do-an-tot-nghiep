using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Admin
{
    public class ShowroomService : IShowroomService
    {
        private readonly IShowroomRepository _showroomRepository;
        private readonly ICarInventoryRepository _inventoryRepository;

        public ShowroomService(IShowroomRepository showroomRepository, ICarInventoryRepository inventoryRepository  )
        {
            _showroomRepository = showroomRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<ShowroomResponseDto>> GetAllShowroomsAsync()
        {
            var showrooms = await _showroomRepository.GetAllAsync();
            return showrooms.Select(s => new ShowroomResponseDto
            {
                ShowroomId = s.ShowroomId,
                Name = s.Name,
                Address = s.Address,
                Hotline = s.Hotline
            });
        }

        public async Task<ShowroomResponseDto?> GetShowroomByIdAsync(int id)
        {
            var showroom = await _showroomRepository.GetByIdAsync(id);
            if (showroom == null) return null;

            return new ShowroomResponseDto
            {
                ShowroomId = showroom.ShowroomId,
                Name = showroom.Name,
                Address = showroom.Address,
                Hotline = showroom.Hotline
            };
        }

        public async Task<ShowroomResponseDto> CreateShowroomAsync(ShowroomRequestDto request)
        {
            var newShowroom = new Showroom
            {
                Name = request.Name,
                Address = request.Address,
                Hotline = request.Hotline
            };

            await _showroomRepository.AddAsync(newShowroom);

            return new ShowroomResponseDto
            {
                ShowroomId = newShowroom.ShowroomId,
                Name = newShowroom.Name,
                Address = newShowroom.Address,
                Hotline = newShowroom.Hotline
            };
        }

        public async Task<bool> UpdateShowroomAsync(int id, ShowroomRequestDto request)
        {
            var showroom = await _showroomRepository.GetByIdAsync(id);
            if (showroom == null) return false;

            showroom.Name = request.Name;
            showroom.Address = request.Address;
            showroom.Hotline = request.Hotline;

            await _showroomRepository.UpdateAsync(showroom);
            return true;
        }

        public async Task<bool> DeleteShowroomAsync(int id)
        {
            var showroom = await _showroomRepository.GetByIdAsync(id);
            if (showroom == null) return false;

            // Xóa cứng vì bảng này độc lập và ít rủi ro
            await _showroomRepository.DeleteAsync(showroom);
            return true;
        }

        public async Task<IEnumerable<ShowroomCarResponseDto>> GetCarsInShowroomAsync(int showroomId)
        {
            // 1. Gọi Repo lấy dữ liệu thô
            var inventories = await _inventoryRepository.GetCarsByShowroomIdAsync(showroomId);

            // 2. Map sang DTO
            return inventories.Select(inv => new ShowroomCarResponseDto
            {
                CarId = inv.CarId,
                Name = inv.Car!.Name,
                //Price = inv.Car.Price,
                Quantity = inv.Quantity,
                DisplayStatus = inv.DisplayStatus,
                // Ưu tiên lấy ảnh chính (IsMainImage = true), nếu không có thì lấy đại tấm đầu tiên
                //MainImageUrl = inv.Car.CarImages.FirstOrDefault(img => img.IsMainImage)?.ImageUrl
                //               ?? inv.Car.CarImages.FirstOrDefault()?.ImageUrl
            });
        }
    }
}
