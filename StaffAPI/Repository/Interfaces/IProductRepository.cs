using EntityFramework.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StaffAPI.Repository.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product, long>
    {
        Task<IEnumerable<Categories>> CateGetAllAsync();
        Task<IEnumerable<Product>> GetTopAsync(Expression<Func<Product, bool>> expression, int top);
    }
}
