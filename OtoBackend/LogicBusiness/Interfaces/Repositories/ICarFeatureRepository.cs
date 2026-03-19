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
        // Nhận vào nguyên một mảng các sợi dây để lưu một phát ăn ngay
        Task AddRangeAsync(IEnumerable<CarFeature> carFeatures);


        // Dùng để cắt đứt hết dây cũ khi Admin xóa xe hoặc muốn cập nhật lại tính năng mới
        Task DeleteByCarIdAsync(int carId);
    }
}