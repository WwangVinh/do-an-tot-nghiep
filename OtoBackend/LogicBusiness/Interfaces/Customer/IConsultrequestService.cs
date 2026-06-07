using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Customer
{
    public interface IConsultRequestService
    {
        Task<(bool Success, string Message)> CreateAsync(ConsultRequestCreateDto dto);
        Task<IEnumerable<object>> GetByPhoneAsync(string phone);
        Task<object?> GetDetailByPhoneAsync(int id, string phone);
        Task<(bool Success, string Message)> CancelByPhoneAsync(int id, string phone, string? reason);
    }
}
