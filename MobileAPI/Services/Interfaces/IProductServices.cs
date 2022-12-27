using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MobileAPI.Services.Interfaces
{
    public interface IProductServices
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Categories>> CateGetAllAsync();
        Task<Product> GetByIdAsync(long Id);
        Task<BaseEntityList<Product>> GetListAsync(
            Expression<Func<Product, bool>> expression,
            Func<Product, object> sort, bool desc,
            int page, int pageSize);
        Task<IEnumerable<Product>> GetTopAsync(Expression<Func<Product, bool>> expression, int top);
    }
}
