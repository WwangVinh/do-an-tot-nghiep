using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        private readonly OtoContext _context;

        public SystemSettingRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<SystemSetting?> GetByKeyAsync(string key)
        {
            return await _context.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == key);
        }

        public async Task UpdateAsync(SystemSetting setting)
        {
            _context.SystemSettings.Update(setting);
            await _context.SaveChangesAsync();
        }
    }
}
