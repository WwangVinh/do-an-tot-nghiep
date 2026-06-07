using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class ConsultRequestRepository : IConsultRequestRepository
    {
        private readonly OtoContext _context;

        public ConsultRequestRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<ConsultRequest?> GetByIdAsync(int id)
        {
            return await _context.ConsultRequests
                .Include(x => x.Car)
                .Include(x => x.Showroom)
                .Include(x => x.User)
                .Include(x => x.CarPricingVersion)
                .Include(x => x.CarColor)
                .FirstOrDefaultAsync(x => x.ConsultRequestId == id);
        }

        public async Task<IEnumerable<ConsultRequest>> GetByPhoneAsync(string phone)
        {
            return await _context.ConsultRequests
                .Include(x => x.Car)
                .Include(x => x.Showroom)
                .Include(x => x.CarPricingVersion)
                .Include(x => x.CarColor)
                .Where(x => x.Phone == phone)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(ConsultRequest entity)
        {
            await _context.ConsultRequests.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ConsultRequest entity)
        {
            _context.ConsultRequests.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<ConsultRequest> Items, int Total)> GetAdminListAsync(
            int page, int pageSize,
            string? search, string? status, string? requestType,
            int? showroomId, int? assignedUserId)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.ConsultRequests
                .Include(x => x.Car)
                .Include(x => x.Showroom)
                .Include(x => x.User)
                .Include(x => x.CarPricingVersion)
                .Include(x => x.CarColor)
                .AsQueryable();

            if (showroomId.HasValue)
                query = query.Where(x => x.ShowroomId == showroomId.Value);

            if (assignedUserId.HasValue)
                query = query.Where(x => x.UserId == assignedUserId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            if (!string.IsNullOrWhiteSpace(requestType))
                query = query.Where(x => x.RequestType == requestType);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();
                query = query.Where(x =>
                    x.CustomerName.Contains(keyword) ||
                    x.Phone.Contains(keyword) ||
                    (x.Car != null && x.Car.Name.Contains(keyword)));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Dictionary<string, int>> CountByStatusAsync(int? showroomId)
        {
            var query = _context.ConsultRequests.AsQueryable();

            if (showroomId.HasValue)
                query = query.Where(x => x.ShowroomId == showroomId.Value);

            var grouped = await query
                .GroupBy(x => x.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var result = new Dictionary<string, int>
            {
                { ConsultStatus.Pending, 0 },
                { ConsultStatus.Consulting, 0 },
                { ConsultStatus.Success, 0 },
                { ConsultStatus.Failed, 0 },
                { ConsultStatus.Cancelled, 0 }
            };

            foreach (var g in grouped)
            {
                result[g.Status] = g.Count;
            }

            return result;
        }
    }
}