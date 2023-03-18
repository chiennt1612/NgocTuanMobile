using StaffAPI.Repository.Interfaces;
using EntityFramework.API.DBContext;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Http;

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
