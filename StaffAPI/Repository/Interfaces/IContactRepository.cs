using EntityFramework.API.Entities;

namespace StaffAPI.Repository.Interfaces
{
    public interface IContactRepository : IGenericRepository<Contact, long>
    {
    }
}
