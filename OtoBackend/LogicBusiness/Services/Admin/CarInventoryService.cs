using CoreEntities.Models;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Admin
{
    public class CarInventoryService : ICarInventoryService
    {
        private readonly ICarInventoryRepository _inventoryRepo;
        private readonly INotificationService _notiService;
        private readonly ICarRepository _carRepo;

        public CarInventoryService(ICarInventoryRepository inventoryRepo, INotificationService notiService, ICarRepository carRepo)
        {
            _inventoryRepo = inventoryRepo;
            _notiService = notiService;
            _carRepo = carRepo;
        }

        public async Task<(bool Success, string Message)> UpdateStockAsync(int carId, int showroomId, int newQuantity, string displayStatus, string? color = null)
        {
            if (newQuantity < 0) return (false, "Số lượng không được âm!");

            var allowedStatuses = new List<string> { "Available", "OnDisplay", "Out of stock" };
            if (!allowedStatuses.Contains(displayStatus))
                return (false, "Trạng thái không hợp lệ! Vui lòng nhập đúng: Available, OnDisplay, hoặc Out of stock.");

            var car = await _carRepo.GetByIdAsync(carId);
            string carName = car != null ? $"{car.Brand} {car.Name}" : $"ID {carId}";
            string colorLabel = !string.IsNullOrWhiteSpace(color) ? $" ({color})" : "";

            var inventory = await _inventoryRepo.GetInventoryAsync(carId, showroomId, color);
            string finalStatus = newQuantity == 0 ? "Out of stock" : displayStatus;

            if (inventory == null)
            {
                await _inventoryRepo.AddInventoryAsync(new CarInventory
                {
                    CarId = carId,
                    ShowroomId = showroomId,
                    Quantity = newQuantity,
                    DisplayStatus = finalStatus,
                    Color = string.IsNullOrWhiteSpace(color) ? null : color.Trim(),
                    UpdatedAt = DateTime.UtcNow
                });
                return (true, $"Thêm mới kho{colorLabel} thành công!");
            }
            else
            {
                int oldQuantity = inventory.Quantity;
                inventory.Quantity = newQuantity;
                inventory.DisplayStatus = finalStatus;
                inventory.Color = string.IsNullOrWhiteSpace(color) ? null : color.Trim();
                inventory.UpdatedAt = DateTime.UtcNow;
                await _inventoryRepo.UpdateInventoryAsync(inventory);

                if (oldQuantity > 0 && newQuantity == 0)
                {
                    await _notiService.CreateNotificationAsync(
                        userId: null,
                        showroomId: showroomId,
                        roleTarget: "Manager,Sales,ShowroomSales",
                        title: "Cảnh báo: Xe đã hết hàng! 🚨",
                        content: $"Mẫu {carName}{colorLabel} tại chi nhánh hiện đã hết hàng. Anh em Sales tạm ngưng nhận cọc nhé!",
                        actionUrl: $"/admin/inventory/detail/{carId}",
                        type: "Inventory"
                    );
                }
                else if (oldQuantity == 0 && newQuantity > 0)
                {
                    await _notiService.CreateNotificationAsync(
                        userId: null,
                        showroomId: showroomId,
                        roleTarget: "Manager,Sales,ShowroomSales",
                        title: "Tin vui: Đã có xe sẵn kho! 📦",
                        content: $"Mẫu {carName}{colorLabel} vừa được bổ sung {newQuantity} chiếc. Anh em gọi khách chốt đơn lẹ nào!",
                        actionUrl: $"/admin/cars/detail/{carId}",
                        type: "Inventory"
                    );
                }

                return (true, "Cập nhật tồn kho thành công!");
            }
        }

        public async Task<IEnumerable<CarInventory>> GetInventoriesByCarIdAsync(int carId)
            => await _inventoryRepo.GetInventoriesByCarIdAsync(carId);

        public async Task<int> GetTotalQuantityAsync(int carId)
            => await _inventoryRepo.GetTotalQuantityByCarIdAsync(carId);

        public async Task<IEnumerable<CarInventory>> GetCarsByShowroomIdAsync(int showroomId)
            => await _inventoryRepo.GetCarsByShowroomIdAsync(showroomId);
    }
}