using CoreEntities.DTOs;
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

    // GET: api/admin/orders?search=abc&status=Pending&paymentStatus=Unpaid&page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAllOrders([FromQuery] OrderQueryParams queryParams)
    {
        var result = await _orderService.GetAdminOrdersAsync(queryParams);
        return Ok(new
        {
            success = true,
            totalItems = result.TotalItems,
            currentPage = result.CurrentPage,
            pageSize = result.PageSize,
            totalPages = result.TotalPages,
            data = result.Data,
        });
    }

    // GET: api/admin/orders/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var result = await _orderService.GetOrderDetailAsync(id);
        if (result == null) return NotFound(new { success = false, message = "Không tìm thấy đơn hàng" });
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
    public async Task<IActionResult> AddPayment(int id, [FromBody] LogicBusiness.DTOs.AddPaymentDto dto)
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