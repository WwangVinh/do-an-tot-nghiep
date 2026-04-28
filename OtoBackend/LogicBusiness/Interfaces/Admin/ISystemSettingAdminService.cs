using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface ISystemSettingAdminService
    {
        Task<string?> GetSettingValueAsync(string key);
        Task<(bool Success, string Message)> UpdateSettingAsync(string key, string value);
    }
}
