using EntityFramework.API.Entities.Identity;

namespace StaffAPI.Repository.Interfaces
{
    public interface IUserDeviceRepository : IGenericARepository<AppUserDevice, long>
    {
    }
}
