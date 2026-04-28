using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales},{AppRoles.Technician}")]
public class OrdersController : ControllerBase
{
    private readonly IOrderAdminService _orderService;

    public OrdersController(IOrderAdminService orderService)
    {
        _orderService = orderService;
    }

    // GET: api/admin/orders
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var result = await _orderService.GetAdminOrdersAsync();
        return Ok(new { success = true, data = result });
    }

    // PUT: api/admin/orders/5/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderRequest request)
    {
        var success = await _orderService.UpdateOrderStatusAsync(id, request.Status, request.AdminNote);

        if (!success) return NotFound(new { success = false, message = "Không tìm thấy đơn hàng" });

        return Ok(new { success = true, message = "Cập nhật thành công" });
    }

    // POST: api/admin/orders/5/payments
    [HttpPost("{id}/payments")]
    public async Task<IActionResult> AddPayment(int id, [FromBody] AddPaymentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _orderService.AddPaymentAsync(id, dto);

        if (!result.Success) return BadRequest(new { success = false, message = result.Message });

        return Ok(new { success = true, message = result.Message });
    }
}

// Lớp phụ để hứng body request của hàm PUT
public class UpdateOrderRequest
{
    public string Status { get; set; } = string.Empty;
    public string AdminNote { get; set; } = string.Empty;
}