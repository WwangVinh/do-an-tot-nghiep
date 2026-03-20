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
        Task<IEnumerable<ShowroomResponseDto>> GetAllShowroomsAsync();
        Task<ShowroomResponseDto?> GetShowroomByIdAsync(int id);
        Task<ShowroomResponseDto> CreateShowroomAsync(ShowroomRequestDto request);
        Task<bool> UpdateShowroomAsync(int id, ShowroomRequestDto request);
        Task<bool> DeleteShowroomAsync(int id);

        Task<IEnumerable<ShowroomCarResponseDto>> GetCarsInShowroomAsync(int showroomId);
    }
}
