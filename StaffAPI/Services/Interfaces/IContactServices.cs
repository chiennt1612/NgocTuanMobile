using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StaffAPI.Services.Interfaces
{
    public interface IContactServices
    {
        Task<Contact> AddAsync(Contact contact);
        Task<Contact> GetByIdAsync(long Id);
        Task<Contact> Update(Contact order);
        Task<BaseEntityList<Contact>> GetListAsync(Expression<Func<Contact, bool>> expression, Func<Contact, object> sort, bool desc, int page, int pageSize);
    }
}
