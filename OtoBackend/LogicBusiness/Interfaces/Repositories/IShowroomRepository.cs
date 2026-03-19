using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IShowroomRepository
    {
        Task<IEnumerable<Showroom>> GetAllAsync();
        Task<Showroom?> GetByIdAsync(int id);
        Task AddAsync(Showroom showroom);
        Task UpdateAsync(Showroom showroom);
        Task DeleteAsync(Showroom showroom);
        Task<bool> CheckExistsAsync(string name, string province, string district, string streetAddress, int? excludeId = null);
    }
}
