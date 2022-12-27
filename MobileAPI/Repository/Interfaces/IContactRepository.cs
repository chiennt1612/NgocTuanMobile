using EntityFramework.API.Entities;

namespace MobileAPI.Repository.Interfaces
{
    public interface IContactRepository : IGenericRepository<Contact, long>
    {
    }
}
