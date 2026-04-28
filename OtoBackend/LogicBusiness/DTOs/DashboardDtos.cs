using System;
using System.Collections.Generic;

namespace LogicBusiness.DTOs
{
    public class DashboardSeriesPointDto
    {
        public string Label { get; set; } = "";
        public decimal Value { get; set; }
    }

    public class DashboardBookingStatusDto
    {
        public string Status { get; set; } = "";
        public int Count { get; set; }
    }

    public class DashboardRecentOrderDto
    {
        public int OrderId { get; set; }
        public string? OrderCode { get; set; }
        public string? CustomerName { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public DateTime? OrderDate { get; set; }
    }

    public class DashboardSummaryDto
    {
        public int Days { get; set; }

        public decimal Revenue { get; set; }
        public int Orders { get; set; }
        public int Bookings { get; set; }
        public int ActiveUsers { get; set; }
        public int ActiveCars { get; set; }
        public int InventoryQuantity { get; set; }

        public List<DashboardSeriesPointDto> RevenueByMonth { get; set; } = new();
        public List<DashboardSeriesPointDto> OrdersByWeek { get; set; } = new();
        public List<DashboardBookingStatusDto> BookingByStatus { get; set; } = new();
        public List<DashboardRecentOrderDto> RecentOrders { get; set; } = new();
    }
}

