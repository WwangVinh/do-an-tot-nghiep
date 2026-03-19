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
        // 1. Lưu một đống thông số lúc Admin tạo/sửa xe
        //Task AddRangeAsync(IEnumerable<CarSpecification> specs);
        Task AddRangeAsync(IEnumerable<CarSpecification> entities);

        // 2. Xóa sạch thông số cũ khi cập nhật xe (giống CarFeature)
        Task DeleteByCarIdAsync(int carId);

        // 3. Hàm "Thần thánh": Lấy danh sách tên thông số không trùng lặp
        Task<IEnumerable<string>> GetDistinctSpecNamesAsync();
    }
}