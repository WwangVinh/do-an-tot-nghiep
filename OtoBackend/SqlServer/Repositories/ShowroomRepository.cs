using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class ShowroomRepository : IShowroomRepository
    {
        private readonly OtoContext _context;

        public ShowroomRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Showroom>> GetAllAsync()
        {
            return await _context.Showrooms.ToListAsync();
        }

        public async Task<Showroom?> GetByIdAsync(int id)
        {
            return await _context.Showrooms.FindAsync(id);
        }

        public async Task AddAsync(Showroom showroom)
        {
            await _context.Showrooms.AddAsync(showroom);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Showroom showroom)
        {
            _context.Showrooms.Update(showroom);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Showroom showroom)
        {
            _context.Showrooms.Remove(showroom);
            await _context.SaveChangesAsync();
        }
    }
}
