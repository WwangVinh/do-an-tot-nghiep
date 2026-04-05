using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreEntities.Models;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface ICarFeatureRepository
    {
        // Nhận vào nguyên một mảng các liên kết giữa xe và tính năng để thêm vào database
        Task AddRangeAsync(IEnumerable<CarFeature> carFeatures);


        // Dùng để cắt đứt hết liên kết khi Admin xóa xe
        Task DeleteByCarIdAsync(int carId);
    }
}