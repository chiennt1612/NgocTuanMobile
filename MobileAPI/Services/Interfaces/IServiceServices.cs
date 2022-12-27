using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MobileAPI.Services.Interfaces
{
    public interface IServiceServices
    {
        Task<IEnumerable<Service>> GetManyAsync(Expression<Func<Service, bool>> where);
        Task<IEnumerable<Service>> GetAllAsync();
        Task<Service> GetByIdAsync(long Id);
        Task<BaseEntityList<Service>> GetListAsync(
            Expression<Func<Service, bool>> expression,
            Func<Service, object> sort, bool desc,
            int page, int pageSize);
    }
}
