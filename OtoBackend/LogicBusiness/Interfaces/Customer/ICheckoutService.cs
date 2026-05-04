using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Customer
{
    public interface ICheckoutService
    {
        Task<string> CreatePaymentLinkAsync(int orderId);
    }
}
