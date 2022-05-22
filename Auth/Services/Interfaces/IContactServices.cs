using EntityFramework.API.Entities;
using System.Threading.Tasks;

namespace Auth.Services.Interfaces
{
    public interface IContactServices
    {
        Task<Contact> AddAsync(Contact contact);
        Task<Contact> GetByIdAsync(long Id);
        Task<Contact> Update(Contact order);
    }
}
