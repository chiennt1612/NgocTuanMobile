using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StaffAPI.Services.Interfaces
{
    public interface IArticleServices
    {
        Task<IList<Article>> GetTopAsync(Expression<Func<Article, bool>> expression, int top);
        Task<IEnumerable<NewsCategories>> CateGetAllAsync();
        Task<IEnumerable<Article>> GetAllAsync();
        Task<Article> GetByIdAsync(long Id);
        Task<BaseEntityList<Article>> GetListAsync(
            Expression<Func<Article, bool>> expression,
            Func<Article, object> sort, bool desc,
            int page, int pageSize);
    }
}
