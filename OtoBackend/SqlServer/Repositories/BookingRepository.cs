using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly OtoContext _context; // Dùng đúng OtoContext của ní

        public BookingRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Booking booking)
        {
            // Thêm đơn hàng mới vào bảng Bookings
            await _context.Bookings.AddAsync(booking);

            // Chốt hạ, lưu xuống SQL Server
            await _context.SaveChangesAsync();
        }
    }
}
