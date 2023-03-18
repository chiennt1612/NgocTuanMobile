using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StaffAPI.Services.Interfaces
{
    public interface IAboutServices
    {
        Task<IEnumerable<About>> GetManyAsync(Expression<Func<About, bool>> where);
        Task<IEnumerable<About>> GetListAsync(IEnumerable<long> Ids);
        Task<IEnumerable<About>> GetAllAsync();
        Task<About> GetByIdAsync(long Id);
        Task<BaseEntityList<About>> GetListAsync(
            Expression<Func<About, bool>> expression,
            Func<About, object> sort, bool desc,
            int page, int pageSize);
    }
}
