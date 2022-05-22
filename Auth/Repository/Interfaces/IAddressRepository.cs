using EntityFramework.API.Entities.Ordering;
//using X.PagedList;

namespace Auth.Repository.Interfaces
{
    public interface IAddressRepository : IGenericRepository<Address, int>
    {
        //Task<BaseEntityList<Address>> GetListAsync(Expression<Func<Address, bool>> expression, Func<Address, object> sort, bool desc, int page, int pageSize);
        //Task<Address> GetByIdAsync(int id);
    }
}
