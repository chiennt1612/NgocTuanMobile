using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Auth.Services.Interfaces
{
    public interface INoticeServices
    {
        Task<IEnumerable<Notice>> GetAllAsync();
        Task<Notice> GetByIdAsync(long Id);
        Task AddManyAsync(IEnumerable<Notice> notice);
        Task<BaseEntityList<Notice>> GetListAsync(
            Expression<Func<Notice, bool>> expression,
            Func<Notice, object> sort, bool desc,
            int page, int pageSize);
        Task<Notice> AddAsync(Notice notice);
        Task DeleteAsync(long id);
    }
}
