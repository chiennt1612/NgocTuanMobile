using EntityFramework.API.DBContext;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Http;
using StaffAPI.Repository.Interfaces;

namespace StaffAPI.Repository
{
    public class ParamSettingRepository : GenericRepository<ParamSetting, long>, IParamSettingRepository
    {
        public ParamSettingRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
              : base(dbContext, contextAccessor)
        {
        }
    }
}
