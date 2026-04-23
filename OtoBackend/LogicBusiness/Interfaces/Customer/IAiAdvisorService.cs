using LogicBusiness.DTOs;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Customer
{
    public interface IAiAdvisorService
    {
        Task<AiAdvisorChatResponseDto> GetReplyAsync(AiAdvisorChatRequestDto request);
    }
}
