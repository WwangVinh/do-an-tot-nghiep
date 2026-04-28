using CoreEntities.Models;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LogicBusiness.Services.Admin
{
    public class CarInventoryService : ICarInventoryService
    {
        private readonly ICarInventoryRepository _inventoryRepo;
        private readonly INotificationService _notiService; // 👈 Thêm súng bắn thông báo
        private readonly ICarRepository _carRepo;


        public CarInventoryService(ICarInventoryRepository inventoryRepo, INotificationService notiService, ICarRepository carRepo)
        {
            _inventoryRepo = inventoryRepo;
            _notiService = notiService;
            _carRepo = carRepo;
        }
        public async Task<(bool Success, string Message)> UpdateStockAsync(int carId, int showroomId, int newQuantity, string displayStatus)
        {
            if (newQuantity < 0) return (false, "Số lượng không được âm!");

            var allowedStatuses = new List<string> { "Available", "OnDisplay", "Out of stock" };
            if (!allowedStatuses.Contains(displayStatus))
            {
                return (false, "Trạng thái không hợp lệ! Vui lòng nhập đúng chữ: Available, OnDisplay, hoặc Out of stock.");
            }

            // Lấy tên xe để thông báo cho đẹp (Nếu ní đã inject ICarRepository)
            var car = await _carRepo.GetByIdAsync(carId);
            string carName = car != null ? $"{car.Brand} {car.Name}" : $"ID {carId}";

            var inventory = await _inventoryRepo.GetInventoryAsync(carId, showroomId);
            string finalStatus = newQuantity == 0 ? "Out of stock" : displayStatus;

            if (inventory == null)
            {
                // TẠO MỚI KHO
                var newInv = new CarInventory
                {
                    CarId = carId,
                    ShowroomId = showroomId,
                    Quantity = newQuantity,
                    DisplayStatus = finalStatus,
                    UpdatedAt = DateTime.Now
                };
                await _inventoryRepo.AddInventoryAsync(newInv);
                return (true, "Thêm mới kho thành công!");
            }
            else
            {
                // CẬP NHẬT KHO
                int oldQuantity = inventory.Quantity; // Lưu lại số lượng cũ để so sánh

                inventory.Quantity = newQuantity;
                inventory.DisplayStatus = finalStatus;
                inventory.UpdatedAt = DateTime.Now;

                await _inventoryRepo.UpdateInventoryAsync(inventory);

                // 👇 LOGIC BẮN THÔNG BÁO Ở ĐÂY 👇

                // Kịch bản 1: Vừa hết sạch hàng
                if (oldQuantity > 0 && newQuantity == 0)
                {
                    await _notiService.CreateNotificationAsync(
                        userId: null,
                        showroomId: showroomId, // Chỉ báo cho chi nhánh bị hết hàng
                        roleTarget: "Manager,Sales,ShowroomSales",
                        title: "Cảnh báo: Xe đã hết hàng! 🚨",
                        content: $"Mẫu {carName} tại chi nhánh hiện đã hết hàng. Anh em Sales tạm ngưng nhận cọc nhé!",
                        actionUrl: $"/admin/inventory/detail/{carId}", // Trỏ về trang quản lý kho
                        type: "Inventory"
                    );
                }
                // Kịch bản 2: Vừa có hàng trở lại (Từ 0 lên một số dương)
                else if (oldQuantity == 0 && newQuantity > 0)
                {
                    await _notiService.CreateNotificationAsync(
                        userId: null,
                        showroomId: showroomId,
                        roleTarget: "Manager,Sales,ShowroomSales",
                        title: "Tin vui: Đã có xe sẵn kho! 📦",
                        content: $"Mẫu {carName} vừa được bổ sung {newQuantity} chiếc. Anh em gọi khách chốt đơn lẹ nào!",
                        actionUrl: $"/admin/cars/detail/{carId}",
                        type: "Inventory"
                    );
                }

                return (true, "Cập nhật tồn kho thành công!");
            }
        }


        public async Task<IEnumerable<CarInventory>> GetInventoriesByCarIdAsync(int carId)
        {
            return await _inventoryRepo.GetInventoriesByCarIdAsync(carId);
        }

        public async Task<int> GetTotalQuantityAsync(int carId)
        {
            return await _inventoryRepo.GetTotalQuantityByCarIdAsync(carId);
        }

        public async Task<IEnumerable<CarInventory>> GetCarsByShowroomIdAsync(int showroomId)
        {
            // Chỉ gọi qua Repo thôi, không dùng trực tiếp _context ở đây nhé
            return await _inventoryRepo.GetCarsByShowroomIdAsync(showroomId);
        }
    }
}
