using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class BookingCreateDto
    {
        public int CarId { get; set; }
        public int ShowroomId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateOnly BookingDate { get; set; }
        public string? BookingTime { get; set; }
        public string? TimeSpan { get; set; }
        public string? Note { get; set; }
        public int? UserId { get; set; }
    }

    public class BookingConsultDto
    {
        public string ConsultNote { get; set; } = null!;
    }

    public class BookingSendTechDto
    {
        public string? TechNote { get; set; }
    }

    public class BookingTechResultDto
    {
        public bool IsApproved { get; set; }
        public string TechNote { get; set; } = null!;
    }

    public class BookingCompleteDto
    {
        public string? ResultNote { get; set; }
    }

    public class BookingNoShowDto
    {
        public string? Reason { get; set; }
    }

    public class BookingCancelDto
    {
        public string CancelReason { get; set; } = null!;
    }

    public class BookingUpdateDto
    {
        public string? NewStatus { get; set; }
        public string? Result { get; set; }
    }

    public class BookingQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? Status { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }

    public class BookingCancelByPhoneDto
    {
        public string Phone { get; set; } = null!;
        public string? Reason { get; set; }
    }

    public static class BookingStatus
    {
        public const string Pending = "Pending";
        public const string Consulted = "Consulted";
        public const string PendingTechCheck = "PendingTechCheck";
        public const string TechApproved = "TechApproved";
        public const string Confirmed = "Confirmed";
        public const string Completed = "Completed";
        public const string NoShow = "NoShow";
        public const string Cancelled = "Cancelled";
    }
}