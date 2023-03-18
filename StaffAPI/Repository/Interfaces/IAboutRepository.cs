using EntityFramework.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StaffAPI.Repository.Interfaces
{
    public interface IAboutRepository : IGenericRepository<About, long>
    {
        Task<IEnumerable<About>> GetListAsync(IEnumerable<long> Ids);
    }
}
