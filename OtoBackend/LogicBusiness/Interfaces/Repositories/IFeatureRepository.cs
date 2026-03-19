using CoreEntities.Models;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IFeatureRepository
    {
        Task<IEnumerable<Feature>> GetAllAsync(string? search = null);
        Task<Feature> GetByIdAsync(int id);
        Task AddAsync(Feature feature);
        Task UpdateAsync(Feature feature);
        Task<List<string>> GetCarNamesUsingFeatureAsync(int featureId);
        Task<bool> CheckExistsAsync(string featureName, int? excludeId = null);
        Task DeleteAsync(int id);
    }
}