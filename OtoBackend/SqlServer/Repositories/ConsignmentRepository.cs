using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class ConsignmentRepository : IConsignmentRepository
    {
        private readonly OtoContext _context;

        public ConsignmentRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Consignment consignment)
        {
            await _context.Consignments.AddAsync(consignment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Consignment consignment)
        {
            _context.Consignments.Update(consignment);
            await _context.SaveChangesAsync();
        }

        public async Task<Consignment?> GetByIdAsync(int id)
        {
            return await _context.Consignments
                .Include(c => c.User) // 👈 Lấy kèm thông tin Khách hàng để Sếp dễ gọi điện
                .FirstOrDefaultAsync(c => c.ConsignmentId == id);
        }

        public async Task<IEnumerable<Consignment>> GetAllAdminAsync()
        {
            return await _context.Consignments
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt) // Mới nhất lên đầu
                .ToListAsync();
        }

        public async Task<IEnumerable<Consignment>> GetByUserIdAsync(int userId)
        {
            return await _context.Consignments
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}