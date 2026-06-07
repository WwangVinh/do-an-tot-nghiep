using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface ICarColorRepository
    {
        // Viết 1 hàm chuyên lấy màu theo Id xe cho tối ưu hiệu năng
        Task<IEnumerable<CarColor>> GetColorsByCarIdAsync(int carId);
    }
}
