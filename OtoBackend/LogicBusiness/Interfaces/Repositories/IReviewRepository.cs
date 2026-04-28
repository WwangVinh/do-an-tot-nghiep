using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        Task AddAsync(Review review);
        Task<List<Review>> GetApprovedReviewsByCarIdAsync(int carId);
        Task SaveChangesAsync();

        Task<bool> IsAlreadyReviewedAsync(string orderCode);
        // 3. Trong IReviewRepository.cs
        Task<int> GetReviewCountByPhoneAsync(string phone);
    }
}
