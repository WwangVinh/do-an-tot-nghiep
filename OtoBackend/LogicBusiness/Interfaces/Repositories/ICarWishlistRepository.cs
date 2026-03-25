using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface ICarWishlistRepository
    {
        Task<CarWishlist?> GetByUserAndCarAsync(int userId, int carId);
        Task<IEnumerable<CarWishlist>> GetMyWishlistAsync(int userId);
        Task AddAsync(CarWishlist wishlist);
        Task DeleteAsync(CarWishlist wishlist);
    }
}
