using EntityFramework.API.Entities;

namespace Auth.Repository.Interfaces
{
    public interface IContactRepository : IGenericRepository<Contact, long>
    {
    }
}
