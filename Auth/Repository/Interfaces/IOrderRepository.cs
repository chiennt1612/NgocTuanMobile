using EntityFramework.API.Entities.Ordering;
//using X.PagedList;

namespace Auth.Repository.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order, long>
    {
        //Task<Order> GetByIdAsync(long id);
        //// Get an entity using delegate
        //Task<BaseEntityList<Order>> GetListAsync(Expression<Func<Order, bool>> expression, Func<Order, object> sort, bool desc, int page, int pageSize);
    }
}
