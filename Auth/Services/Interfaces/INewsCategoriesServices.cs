using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Auth.Services.Interfaces
{
    public interface INewsCategoriesServices
    {
        Task<IEnumerable<NewsCategories>> GetAllAsync();
        Task<NewsCategories> GetByIdAsync(long Id);
        Task<BaseEntityList<NewsCategories>> GetListAsync(
            Expression<Func<NewsCategories, bool>> expression,
            Func<NewsCategories, object> sort, bool desc,
            int page, int pageSize);
    }
}
