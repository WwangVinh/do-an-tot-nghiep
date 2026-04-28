using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface ISystemSettingRepository
    {
        Task<SystemSetting?> GetByKeyAsync(string key);
        Task UpdateAsync(SystemSetting setting);
    }
}
