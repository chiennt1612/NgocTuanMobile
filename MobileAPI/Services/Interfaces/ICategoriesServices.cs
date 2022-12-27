using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MobileAPI.Services.Interfaces
{
    public interface ICategoriesServices
    {
        Task<IEnumerable<Categories>> GetAllAsync();
        Task<Categories> GetByIdAsync(long Id);
        Task<BaseEntityList<Categories>> GetListAsync(
            Expression<Func<Categories, bool>> expression,
            Func<Categories, object> sort, bool desc,
            int page, int pageSize);
    }
}
