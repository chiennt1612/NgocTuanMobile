using EntityFramework.API.Entities.Ordering;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MobileAPI.Repository.Interfaces
{
    public interface IOrderStatusRepository
    {
        Task<IEnumerable<OrderStatus>> GetAllAsync();
        Task<OrderStatus> GetByIdAsync(int id);
        // Get an entity using delegate
        Task<OrderStatus> GetAsync(Expression<Func<OrderStatus, bool>> where);
    }
}
