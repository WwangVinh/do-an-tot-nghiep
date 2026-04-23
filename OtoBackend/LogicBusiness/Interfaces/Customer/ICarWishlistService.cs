using LogicBusiness.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Customer
{
    public interface ICarWishlistService
    {
        Task<(bool Success, string Message, bool IsHearted)> ToggleWishlistAsync(int userId, int carId);
        Task<IEnumerable<WishlistResponseDto>> GetMyWishlistAsync(int userId);
    }
}
