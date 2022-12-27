using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MobileAPI.Services.Interfaces
{
    public interface IFAQServices
    {
        Task<IList<FAQ>> GetTopAsync(Expression<Func<FAQ, bool>> expression, int top);
        Task<IEnumerable<FAQ>> GetAllAsync();
        Task<FAQ> GetByIdAsync(long Id);
        Task<BaseEntityList<FAQ>> GetListAsync(
            Expression<Func<FAQ, bool>> expression,
            Func<FAQ, object> sort, bool desc,
            int page, int pageSize);
    }
}
