using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicBusiness.Interfaces.Repositories;


namespace SqlServer.Repositories
{
    public class CarSpecificationRepository : ICarSpecificationRepository
    {
        private readonly OtoContext _context;

        public CarSpecificationRepository(OtoContext context)
        {
            _context = context;
        }


        public async Task AddRangeAsync(IEnumerable<CarSpecification> entities)
        {
            if (entities == null || !entities.Any()) return;
            await _context.CarSpecifications.AddRangeAsync(entities);
            await _context.SaveChangesAsync(); // Đây là nhát búa chốt hạ để data vào DB
        }

        //public async Task AddRangeAsync(IEnumerable<CarSpecification> specs)
        //{
        //    await _context.CarSpecifications.AddRangeAsync(specs);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task DeleteByCarIdAsync(int carId)
        //{
        //    var itemsToDelete = await _context.CarSpecifications
        //                                      .Where(x => x.CarId == carId)
        //                                      .ToListAsync();

        //    if (itemsToDelete.Any())
        //    {
        //        _context.CarSpecifications.RemoveRange(itemsToDelete);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        public async Task DeleteByCarIdAsync(int carId)
        {
            var specs = await _context.CarSpecifications.Where(x => x.CarId == carId).ToListAsync();
            if (specs.Any())
            {
                _context.CarSpecifications.RemoveRange(specs);
                await _context.SaveChangesAsync();
            }
        }

        // Dùng Distinct() để lọc trùng. VD: 10 xe có "Công suất", nó chỉ lấy 1 chữ "Công suất"
        public async Task<IEnumerable<string>> GetDistinctSpecNamesAsync()
        {
            return await _context.CarSpecifications
                                 .Select(x => x.SpecName)
                                 .Distinct()
                                 .ToListAsync();
        }
    }
}
