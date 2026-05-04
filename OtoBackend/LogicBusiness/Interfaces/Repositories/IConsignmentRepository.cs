using CoreEntities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IConsignmentRepository
    {
        Task AddAsync(Consignment consignment);
        Task UpdateAsync(Consignment consignment);
        Task<Consignment?> GetByIdAsync(int id);
        Task<IEnumerable<Consignment>> GetAllAdminAsync();
        Task<Consignment?> GetPendingByPhoneAndCarAsync(string phone, string brand, string model, int year);
    }
}