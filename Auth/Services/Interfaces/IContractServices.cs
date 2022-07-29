using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Auth.Services.Interfaces
{
    public interface IContractServices
    {
        Task<IEnumerable<Contract>> GetAllAsync();
        Task<Contract> GetByIdAsync(long Id);
        Task AddManyAsync(IEnumerable<Contract> notice);
        Task<BaseEntityList<Contract>> GetListAsync(
            Expression<Func<Contract, bool>> expression,
            Func<Contract, object> sort, bool desc,
            int page, int pageSize);
        Task<Contract> AddAsync(Contract notice);
        Task DeleteAsync(long id);

        Task<Contract> GetAsync(Expression<Func<Contract, bool>> where);
        Task<IEnumerable<Contract>> GetManyAsync(Expression<Func<Contract, bool>> where);
    }
}
