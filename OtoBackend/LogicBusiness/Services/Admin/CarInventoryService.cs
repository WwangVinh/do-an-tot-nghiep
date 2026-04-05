using CoreEntities.Models;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
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


        public CarInventoryService(ICarInventoryRepository inventoryRepo)
        {
            _inventoryRepo = inventoryRepo;

        }
        public async Task<(bool Success, string Message)> UpdateStockAsync(int carId, int showroomId, int newQuantity, string displayStatus)
        {
            if (newQuantity < 0) return (false, "Số lượng không được âm!");

            var allowedStatuses = new List<string> { "Available", "OnDisplay", "Out of stock" };
            if (!allowedStatuses.Contains(displayStatus))
            {
                return (false, "Trạng thái không hợp lệ! Vui lòng nhập đúng chữ: Available, OnDisplay, hoặc Out of stock.");
            }

            var inventory = await _inventoryRepo.GetInventoryAsync(carId, showroomId);

            // Xử lý logic tự động: Nếu gõ số lượng = 0 thì ép kiểu về Out of stock luôn, Admin chọn gì cũng mặc kệ
            string finalStatus = newQuantity == 0 ? "Out of stock" : displayStatus;

            if (inventory == null)
            {
                // Tạo mới
                var newInv = new CarInventory
                {
                    CarId = carId,
                    ShowroomId = showroomId,
                    Quantity = newQuantity,
                    DisplayStatus = finalStatus, // 👈 Lấy trạng thái Admin truyền vào
                    UpdatedAt = DateTime.Now
                };
                await _inventoryRepo.AddInventoryAsync(newInv);
                return (true, "Thêm mới kho thành công!");
            }
            else
            {
                // Cập nhật
                inventory.Quantity = newQuantity;
                inventory.DisplayStatus = finalStatus; // 👈 Lấy trạng thái Admin truyền vào
                inventory.UpdatedAt = DateTime.Now;

                await _inventoryRepo.UpdateInventoryAsync(inventory);
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
