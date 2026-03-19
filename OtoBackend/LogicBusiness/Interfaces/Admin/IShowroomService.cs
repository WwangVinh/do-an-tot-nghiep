using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface IShowroomService
    {
        Task<IEnumerable<ShowroomDto>> GetAllShowroomsAsync();
        Task<ShowroomDto?> GetShowroomByIdAsync(int id);
        Task<(bool Success, string Message)> CreateShowroomAsync(ShowroomCreateDto dto);
        Task<(bool Success, string Message)> UpdateShowroomAsync(int id, ShowroomUpdateDto dto);
        Task<(bool Success, string Message)> DeleteShowroomAsync(int id);
    }
}
