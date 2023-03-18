using EntityFramework.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StaffAPI.Repository.Interfaces
{
    public interface IArticleRepository : IGenericRepository<Article, long>
    {
        Task<IEnumerable<NewsCategories>> CateGetAllAsync();
        Task<IList<Article>> GetTopAsync(Expression<Func<Article, bool>> expression, int top);
    }
}
