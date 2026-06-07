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

        // Các trạng thái hiển thị hợp lệ — gom thành const cho dễ maintain
        private static readonly HashSet<string> AllowedDisplayStatuses = new()
        {
            "Available", "OnDisplay", "Out of stock"
        };

        public CarInventoryService(
            ICarInventoryRepository inventoryRepo,
            INotificationService notiService,
            ICarRepository carRepo)
        {
            _inventoryRepo = inventoryRepo;
            _notiService = notiService;
            _carRepo = carRepo;
        }

        public async Task<(bool Success, string Message)> UpdateStockAsync(
            int carId,
            int showroomId,
            int newQuantity,
            string displayStatus,
            int? carColorId = null)
        {
            // ===== 1. VALIDATE INPUT CƠ BẢN =====
            if (newQuantity < 0)
                return (false, "Số lượng không được âm!");

            if (!AllowedDisplayStatuses.Contains(displayStatus))
                return (false, "Trạng thái không hợp lệ! Vui lòng nhập đúng: Available, OnDisplay, hoặc Out of stock.");

            // ===== 2. LẤY XE + VALIDATE TỒN TẠI =====
            // ⚠️ LƯU Ý: CarRepository.GetByIdAsync PHẢI Include(c => c.CarColors)
            // không thì validate màu bên dưới sẽ fail oan!
            var car = await _carRepo.GetByIdAsync(carId);
            if (car == null)
                return (false, $"Không tìm thấy xe với ID {carId}!");

            string carName = $"{car.Brand} {car.Name}";

            // ===== 3. VALIDATE MÀU XE (nếu có truyền) =====
            string colorLabel = "";
            if (carColorId.HasValue)
            {
                if (car.CarColors == null || !car.CarColors.Any())
                    return (false, $"Xe {carName} chưa có cấu hình màu nào, không thể gán màu cho lô hàng!");

                var colorObj = car.CarColors.FirstOrDefault(c => c.CarColorId == carColorId.Value);
                if (colorObj == null)
                    return (false, $"Màu (ID {carColorId.Value}) không thuộc về xe {carName}. Vui lòng chọn lại!");

                if (!colorObj.IsActive)
                    return (false, $"Màu '{colorObj.ColorName}' đã bị tắt, không thể nhập kho cho màu này!");

                colorLabel = $" (Màu {colorObj.ColorName})";
            }

            // ===== 4. UPSERT INVENTORY =====
            // ⚠️ Để chống race condition (2 request cùng lúc tạo trùng record),
            // nên đặt UNIQUE INDEX trên (CarId, ShowroomId, CarColorId) ở DB.
            var inventory = await _inventoryRepo.GetInventoryAsync(carId, showroomId, carColorId);
            string finalStatus = newQuantity == 0 ? "Out of stock" : displayStatus;

            if (inventory == null)
            {
                // Tạo mới
                await _inventoryRepo.AddInventoryAsync(new CarInventory
                {
                    CarId = carId,
                    ShowroomId = showroomId,
                    Quantity = newQuantity,
                    DisplayStatus = finalStatus,
                    CarColorId = carColorId,
                    UpdatedAt = DateTime.UtcNow
                });

                // Nếu tạo mới mà có hàng luôn -> báo "đã có xe"
                if (newQuantity > 0)
                {
                    await NotifyStockInAsync(showroomId, carId, carName, colorLabel, newQuantity);
                }

                return (true, $"Thêm mới kho{colorLabel} thành công!");
            }

            // Cập nhật record cũ
            int oldQuantity = inventory.Quantity;
            inventory.Quantity = newQuantity;
            inventory.DisplayStatus = finalStatus;
            inventory.CarColorId = carColorId;
            inventory.UpdatedAt = DateTime.UtcNow;
            await _inventoryRepo.UpdateInventoryAsync(inventory);

            // ===== 5. GỬI NOTIFICATION KHI ĐỔI TRẠNG THÁI HÀNG =====
            if (oldQuantity > 0 && newQuantity == 0)
            {
                await NotifyOutOfStockAsync(showroomId, carId, carName, colorLabel);
            }
            else if (oldQuantity == 0 && newQuantity > 0)
            {
                await NotifyStockInAsync(showroomId, carId, carName, colorLabel, newQuantity);
            }

            return (true, "Cập nhật tồn kho thành công!");
        }

        // ===================== HELPERS =====================

        private Task NotifyOutOfStockAsync(int showroomId, int carId, string carName, string colorLabel)
        {
            return _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: showroomId,
                roleTarget: "Manager,Sales,ShowroomSales",
                title: "Cảnh báo: Xe đã hết hàng! 🚨",
                content: $"Mẫu {carName}{colorLabel} tại chi nhánh hiện đã hết hàng. Anh em Sales tạm ngưng nhận cọc nhé!",
                actionUrl: $"/admin/inventory/detail/{carId}",
                type: "Inventory"
            );
        }

        private Task NotifyStockInAsync(int showroomId, int carId, string carName, string colorLabel, int newQuantity)
        {
            return _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: showroomId,
                roleTarget: "Manager,Sales,ShowroomSales",
                title: "Tin vui: Đã có xe sẵn kho! 📦",
                content: $"Mẫu {carName}{colorLabel} vừa được bổ sung {newQuantity} chiếc. Anh em gọi khách chốt đơn lẹ nào!",
                actionUrl: $"/admin/cars/detail/{carId}",
                type: "Inventory"
            );
        }

        // ===================== CÁC METHOD KHÁC (giữ nguyên) =====================

        public async Task<IEnumerable<CarInventory>> GetInventoriesByCarIdAsync(int carId)
            => await _inventoryRepo.GetInventoriesByCarIdAsync(carId);

        public async Task<int> GetTotalQuantityAsync(int carId)
            => await _inventoryRepo.GetTotalQuantityByCarIdAsync(carId);

        public async Task<IEnumerable<CarInventory>> GetCarsByShowroomIdAsync(int showroomId)
            => await _inventoryRepo.GetCarsByShowroomIdAsync(showroomId);
    }
}