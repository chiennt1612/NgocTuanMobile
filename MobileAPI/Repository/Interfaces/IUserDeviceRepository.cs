using EntityFramework.API.Entities.Identity;

namespace MobileAPI.Repository.Interfaces
{
    public interface IUserDeviceRepository : IGenericARepository<AppUserDevice, long>
    {
    }
}
