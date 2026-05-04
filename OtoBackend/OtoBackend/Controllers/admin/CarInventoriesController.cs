using Microsoft.AspNetCore.Mvc;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;

namespace OtoBackend.Controllers.admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}")]
    public class CarInventoriesController : ControllerBase
    {
        private readonly ICarInventoryService _inventoryService;

        public CarInventoriesController(ICarInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("car/{carId}")]
        public async Task<IActionResult> GetInventoriesByCar(int carId)
        {
            var inventories = await _inventoryService.GetInventoriesByCarIdAsync(carId);
            var total = await _inventoryService.GetTotalQuantityAsync(carId);

            return Ok(new
            {
                CarId = carId,
                TotalQuantity = total,
                Details = inventories.Select(i => new
                {
                    i.ShowroomId,
                    ShowroomName = i.Showroom?.Name ?? "Không xác định",
                    Province = i.Showroom?.Province ?? "Chưa rõ",
                    i.Quantity,
                    i.DisplayStatus,
                    i.Color
                })
            });
        }

        [HttpPut("update-stock")]
        public async Task<IActionResult> UpdateStock([FromBody] UpdateStockDto request)
        {
            var result = await _inventoryService.UpdateStockAsync(
                request.CarId,
                request.ShowroomId,
                request.Quantity,
                request.DisplayStatus,
                request.Color
            );

            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }
    }
}