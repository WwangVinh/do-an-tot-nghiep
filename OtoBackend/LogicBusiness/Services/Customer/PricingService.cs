using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;

namespace LogicBusiness.Services.Customer
{
    public class PricingService : IPricingService
    {
        private readonly ICarRepository _carRepository;
        private readonly ICarPricingVersionRepository _pricingRepository;

        public PricingService(ICarRepository carRepository, ICarPricingVersionRepository pricingRepository)
        {
            _carRepository = carRepository;
            _pricingRepository = pricingRepository;
        }

        public async Task<IEnumerable<PricingCarDto>> GetPricingCarsAsync()
        {
            // 1) Lấy danh sách xe từ bảng Cars (tên + ảnh)
            var cars = await _carRepository.GetCarsForPricingAsync();

            // 2) Lấy danh sách version/giá từ bảng CarPricingVersions
            var versions = await _pricingRepository.GetActiveAsync();
            var versionMap = versions
                .GroupBy(x => x.CarId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => x.SortOrder).ThenBy(x => x.PricingVersionId).ToList()
                );

            // 3) Ghép dữ liệu: Cars + CarPricingVersions
            // Chỉ trả các xe có ít nhất 1 version giá để FE render bảng.
            return cars
                .Where(c => versionMap.ContainsKey(c.CarId) && versionMap[c.CarId].Any())
                .Select(c =>
                {
                    var carVersions = versionMap[c.CarId];
                    return new PricingCarDto
                    {
                        CarId = c.CarId,
                        Name = c.Name,
                        ImageKey = InferImageKey(c.Name),
                        ImageUrl = c.ImageUrl,
                        Versions = carVersions.Select(MapVersionDto).ToList()
                    };
                })
                .OrderBy(x => x.Name)
                .ToList();
        }

        private static PricingVersionDto MapVersionDto(CarPricingVersion entity)
        {
            return new PricingVersionDto
            {
                PricingVersionId = entity.PricingVersionId,
                Name = entity.VersionName,
                PriceVnd = entity.PriceVnd,
                SortOrder = entity.SortOrder,
                IsActive = entity.IsActive
            };
        }

        private static string InferImageKey(string? carName)
        {
            var normalized = (carName ?? string.Empty).ToLowerInvariant();

            if (normalized.Contains("vf3")) return "vf3";
            if (normalized.Contains("vf5")) return "vf5";
            if (normalized.Contains("vf6")) return "vf6";
            if (normalized.Contains("vf8")) return "vf8";
            if (normalized.Contains("vf9")) return "vf9";
            return "vf7";
        }
    }
}
