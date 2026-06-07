using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class CarColorRepository : ICarColorRepository
    {
        private readonly OtoContext _context;

        public CarColorRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CarColor>> GetColorsByCarIdAsync(int carId)
        {
            // Chỉ lấy các màu đang Active của đúng cái xe đó
            return await _context.CarColors
                .Where(c => c.CarId == carId && c.IsActive)
                .ToListAsync();
        }
    }
}