using EntityFramework.API.Entities.Identity;

namespace Auth.Repository.Interfaces
{
    public interface IUserDeviceRepository : IGenericARepository<AppUserDevice, long>
    {
    }
}
