using CoreEntities.Models;
using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LogicBusiness.Interfaces.Admin
{
    public interface IFeatureService
    {
        Task<IEnumerable<Feature>> GetAllFeaturesAsync(string? search = null);
        Task<Feature> GetFeatureByIdAsync(int id);
        Task<(bool IsSuccess, string Message)> CreateFeatureAsync(FeatureCreateDto dto);

        Task<(bool IsSuccess, string Message)> UpdateFeatureAsync(int id, FeatureUpdateDto dto);

        Task<(bool IsSuccess, string Message)> DeleteFeatureAsync(int id);
    }
}