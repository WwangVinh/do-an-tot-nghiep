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
                .FirstOrDefaultAsync(c => c.ConsignmentId == id);
        }

        public async Task<IEnumerable<Consignment>> GetAllAdminAsync()
        {
            return await _context.Consignments
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Consignment?> GetPendingByPhoneAndCarAsync(string phone, string brand, string model, int year)
        {
            return await _context.Consignments
                .Where(c =>
                    c.GuestPhone == phone &&
                    c.Brand.ToLower() == brand.ToLower() &&
                    c.Model.ToLower() == model.ToLower() &&
                    c.Year == year &&
                    (c.Status == "Pending" || c.Status == "Appraising"))
                .FirstOrDefaultAsync();
        }
    }
}