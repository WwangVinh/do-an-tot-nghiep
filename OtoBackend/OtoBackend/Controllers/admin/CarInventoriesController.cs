using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;
using LogicBusiness.DTOs;

namespace OtoBackend.Controllers.admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class CarInventoriesController : ControllerBase
    {
        private readonly ICarInventoryService _inventoryService;

        public CarInventoriesController(ICarInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// Xem chi tiết số lượng xe phân bổ ở các Showroom
        /// </summary>
        [HttpGet("car/{carId}")]
        public async Task<IActionResult> GetInventoriesByCar(int carId)
        {
            var inventories = await _inventoryService.GetInventoriesByCarIdAsync(carId);
            var total = await _inventoryService.GetTotalQuantityAsync(carId);

            // Xào nấu lại dữ liệu trả về cho Frontend dễ đọc
            var result = new
            {
                CarId = carId,
                TotalQuantity = total,
                Details = inventories.Select(i => new
                {
                    i.ShowroomId,
                    ShowroomName = i.Showroom?.Name ?? "Không xác định",
                    Province = i.Showroom?.Province ?? "Chưa rõ",
                    i.Quantity,
                    i.DisplayStatus
                })
            };

            return Ok(result);
        }

        /// <summary>
        /// Cập nhật số lượng xe trong kho (Nhập thêm hoặc Bán đi)
        /// </summary>
        [HttpPut("update-stock")]
        public async Task<IActionResult> UpdateStock([FromBody] UpdateStockDto request)
        {
            var result = await _inventoryService.UpdateStockAsync(request.CarId, request.ShowroomId, request.Quantity, request.DisplayStatus);

            if (result.Success)
            {
                return Ok(new { message = result.Message });
            }

            return BadRequest(new { message = result.Message });
        }
    }
}
