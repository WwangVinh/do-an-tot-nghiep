using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface ICarSpecificationService
    {
        Task<IEnumerable<string>> GetSuggestedSpecNamesAsync();
    }
}
