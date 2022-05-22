using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Auth.Services.Interfaces
{
    public interface IParamSettingServices
    {
        Task<IEnumerable<ParamSetting>> GetAllAsync();
        Task<ParamSetting> GetByIdAsync(long Id);
        Task<BaseEntityList<ParamSetting>> GetListAsync(
            Expression<Func<ParamSetting, bool>> expression,
            Func<ParamSetting, object> sort, bool desc,
            int page, int pageSize);
    }
}
