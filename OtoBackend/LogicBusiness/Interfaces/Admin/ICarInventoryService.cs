using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface ICarInventoryService
    {
        // Hàm này để Admin cập nhật lại số lượng xe trong kho (Nhập thêm hoặc bán bớt)
        Task<(bool Success, string Message)> UpdateStockAsync(int carId, int showroomId, int newQuantity);

        // Lấy chi tiết xe này đang nằm ở những showroom nào
        Task<IEnumerable<CarInventory>> GetInventoriesByCarIdAsync(int carId);

        // Tính tổng số lượng xe đang có trên toàn quốc
        Task<int> GetTotalQuantityAsync(int carId);

        Task<IEnumerable<CarInventory>> GetCarsByShowroomIdAsync(int showroomId);
    }
}
