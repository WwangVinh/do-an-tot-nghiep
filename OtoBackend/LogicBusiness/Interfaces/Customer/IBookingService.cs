using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Customer
{
    public interface IBookingService
    {
        // Hàm xử lý logic: Đặt lịch / Đặt cọc và Trừ kho chống hớt tay trên
        Task<(bool Success, string Message)> CreateBookingAsync(BookingCreateDto dto);
    }
}
