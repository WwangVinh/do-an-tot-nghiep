using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;

namespace LogicBusiness.Services.Admin
{
    public class CarSpecificationService : ICarSpecificationService
    {
        private readonly ICarSpecificationRepository _specRepo;

        public CarSpecificationService(ICarSpecificationRepository specRepo)
        {
            _specRepo = specRepo;
        }

        public async Task<IEnumerable<string>> GetSuggestedSpecNamesAsync()
        {
            // Ní có thể thêm logic ở đây sau này nếu cần, hiện tại cứ pass thẳng qua Repo
            return await _specRepo.GetDistinctSpecNamesAsync();
        }
    }
}