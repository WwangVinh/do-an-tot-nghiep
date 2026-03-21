using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IBookingRepository
    {
        
        Task AddAsync(Booking booking);

        // Sau này muốn làm thêm chức năng Hủy đơn, Xem lịch sử đơn... 
        // thì mình sẽ khai báo thêm ở đây sau nha.
    }
}
