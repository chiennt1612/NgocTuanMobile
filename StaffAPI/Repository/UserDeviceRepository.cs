using StaffAPI.Repository.Interfaces;
using EntityFramework.API.DBContext;
using EntityFramework.API.Entities.Identity;
using Microsoft.AspNetCore.Http;

namespace StaffAPI.Repository
{
    public class UserDeviceRepository : GenericARepository<AppUserDevice, long>, IUserDeviceRepository
    {
        private readonly UserDbContext _context;
        public UserDeviceRepository(UserDbContext dbContext, IHttpContextAccessor contextAccessor)
            : base(dbContext, contextAccessor)
        {
            _context = dbContext;
        }
    }
}
