using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly OtoContext _context;

        public DashboardRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(int days, string userRole, int? userShowroomId)
        {
            var safeDays = days <= 0 ? 30 : Math.Min(days, 365);
            var now = DateTime.Now;
            var from = now.AddDays(-safeDays);

            int? showroomScope = (userRole == "Admin") ? null : userShowroomId;

            // Orders: last N days (by OrderDate)
            var ordersQuery = _context.Orders.AsNoTracking()
                .Include(o => o.Staff)
                .Where(o => (o.OrderDate ?? now) >= from);

            var revenue = await ordersQuery
                .Where(o => o.PaymentStatus == "Paid")
                .SumAsync(o => (decimal?)o.FinalAmount) ?? 0m;

            var ordersCount = await ordersQuery.CountAsync();

            // Bookings: last N days (by CreatedAt), with showroom scope for non-admin
            var bookingsQuery = _context.Bookings.AsNoTracking().Where(b => (b.CreatedAt ?? now) >= from);
            if (showroomScope.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(b => b.ShowroomId == showroomScope.Value);
            }

            var bookingsCount = await bookingsQuery.CountAsync();
            var bookingByStatus = await bookingsQuery
                .GroupBy(b => b.Status ?? "Unknown")
                .Select(g => new DashboardBookingStatusDto { Status = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            // Users, Cars, Inventory snapshot
            var activeUsers = await _context.Users.AsNoTracking()
                .Where(u => (u.Status ?? "Active") == "Active" && u.DeletedAt == null)
                .CountAsync();

            var activeCars = await _context.Cars.AsNoTracking().Where(c => c.IsDeleted == false).CountAsync();

            var inventoryQuery = _context.CarInventories.AsNoTracking().AsQueryable();
            if (showroomScope.HasValue)
            {
                inventoryQuery = inventoryQuery.Where(ci => ci.ShowroomId == showroomScope.Value);
            }
            var inventoryQty = await inventoryQuery.SumAsync(ci => (int?)ci.Quantity) ?? 0;

            // Revenue by month (last 12 months)
            var startMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-11);
            var endMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1);

            var revenueByMonthRaw = await _context.Orders.AsNoTracking()
                .Where(o => o.PaymentStatus == "Paid" && o.OrderDate != null && o.OrderDate >= startMonth && o.OrderDate < endMonth)
                .GroupBy(o => new { o.OrderDate!.Value.Year, o.OrderDate!.Value.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Value = g.Sum(x => (decimal?)x.FinalAmount) ?? 0m
                })
                .ToListAsync();

            var revenueByMonth = new List<DashboardSeriesPointDto>();
            for (int i = 0; i < 12; i++)
            {
                var m = startMonth.AddMonths(i);
                var v = revenueByMonthRaw.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month)?.Value ?? 0m;
                revenueByMonth.Add(new DashboardSeriesPointDto
                {
                    Label = m.ToString("MM/yyyy", CultureInfo.InvariantCulture),
                    Value = v
                });
            }

            // Orders by week (last 8 weeks)
            DateTime StartOfWeek(DateTime d)
            {
                var day = (int)d.DayOfWeek; // Sunday=0
                var delta = (day == 0 ? 6 : day - 1);
                return d.Date.AddDays(-delta);
            }

            var startWeek = StartOfWeek(now).AddDays(-7 * 7);
            var endWeek = StartOfWeek(now).AddDays(7);

            // EF can't translate custom StartOfWeek inside GroupBy reliably => load minimal data then group in-memory.
            var ordersInWeeks = await _context.Orders.AsNoTracking()
                .Where(o => o.OrderDate != null && o.OrderDate >= startWeek && o.OrderDate < endWeek)
                .Select(o => o.OrderDate!.Value)
                .ToListAsync();

            var ordersByWeekRaw = ordersInWeeks
                .GroupBy(StartOfWeek)
                .Select(g => new { WeekStart = g.Key, Count = g.Count() })
                .ToList();

            var ordersByWeek = new List<DashboardSeriesPointDto>();
            for (int i = 0; i < 8; i++)
            {
                var wk = StartOfWeek(now).AddDays(-7 * (7 - i));
                var v = ordersByWeekRaw.FirstOrDefault(x => x.WeekStart == wk)?.Count ?? 0;
                ordersByWeek.Add(new DashboardSeriesPointDto
                {
                    Label = $"{wk:dd/MM}",
                    Value = v
                });
            }

            // Recent orders
            var recentOrders = await _context.Orders.AsNoTracking()
                .Include(o => o.Staff)
                .OrderByDescending(o => o.OrderDate ?? DateTime.MinValue)
                .ThenByDescending(o => o.OrderId)
                .Take(6)
                .Select(o => new DashboardRecentOrderDto
                {
                    OrderId = o.OrderId,
                    OrderCode = o.OrderCode,
                    CustomerName = o.Staff != null ? o.Staff.FullName : null,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    Amount = o.FinalAmount != 0m ? o.FinalAmount : (o.TotalAmount ?? 0m),
                    OrderDate = o.OrderDate
                })
                .ToListAsync();

            return new DashboardSummaryDto
            {
                Days = safeDays,
                Revenue = revenue,
                Orders = ordersCount,
                Bookings = bookingsCount,
                ActiveUsers = activeUsers,
                ActiveCars = activeCars,
                InventoryQuantity = inventoryQty,
                RevenueByMonth = revenueByMonth,
                OrdersByWeek = ordersByWeek,
                BookingByStatus = bookingByStatus,
                RecentOrders = recentOrders
            };
        }
    }
}

