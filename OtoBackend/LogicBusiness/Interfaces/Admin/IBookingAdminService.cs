using LogicBusiness.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface IBookingAdminService
    {
        Task<object> GetBookingsForAdminAsync(
            int page, int pageSize, string? search, string? status,
            string userRole, int? userShowroomId);

        Task<object?> GetBookingDetailAsync(
            int bookingId, string userRole, int? userShowroomId);

        Task<(bool Success, string Message)> MarkAsConsultedAsync(
            int bookingId, BookingConsultDto dto,
            string userRole, int? userShowroomId);

        Task<(bool Success, string Message)> SendToTechCheckAsync(
            int bookingId, BookingSendTechDto dto,
            string userRole, int? userShowroomId);

        Task<(bool Success, string Message)> SubmitTechResultAsync(
            int bookingId, BookingTechResultDto dto,
            string userRole, int? userShowroomId);

        Task<(bool Success, string Message)> ConfirmBookingAsync(
            int bookingId, string userRole, int? userShowroomId);

        Task<(bool Success, string Message)> CompleteBookingAsync(
            int bookingId, string? resultNote,
            string userRole, int? userShowroomId);

        Task<(bool Success, string Message)> MarkNoShowAsync(
            int bookingId, BookingNoShowDto dto,
            string userRole, int? userShowroomId);

        Task<(bool Success, string Message)> CancelBookingByAdminAsync(
            int bookingId, BookingCancelDto dto,
            string userRole, int? userShowroomId);

        Task<object> GetPendingTechCheckAsync(string userRole, int? userShowroomId);

        Task<Dictionary<string, int>> GetBookingStatsAsync(string userRole, int? userShowroomId);
    }
}