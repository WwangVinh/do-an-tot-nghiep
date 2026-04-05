using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicBusiness.DTOs;
using CoreEntities.Models;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface ICarSpecificationRepository
    {
        // Lưu một đống thông số lúc Admin tạo/sửa xe
        //Task AddRangeAsync(IEnumerable<CarSpecification> specs);
        Task AddRangeAsync(IEnumerable<CarSpecification> entities);

        // Xóa sạch thông số cũ khi cập nhật xe (giống CarFeature)
        Task DeleteByCarIdAsync(int carId);

        // Hàm Lấy danh sách tên thông số không trùng lặp
        Task<IEnumerable<string>> GetDistinctSpecNamesAsync();
    }
}