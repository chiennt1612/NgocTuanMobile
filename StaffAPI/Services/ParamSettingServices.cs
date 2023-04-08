using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using Microsoft.Extensions.Logging;
using StaffAPI.Repository.Interfaces;
using StaffAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StaffAPI.Services
{
    public class ParamSettingServices : IParamSettingServices
    {
        private IUnitOfWork unitOfWork;
        private ILogger<ParamSettingServices> ilogger;
        public static IEnumerable<ParamSetting> _GetAll; // cache tạm thời
        public ParamSettingServices(IUnitOfWork unitOfWork, ILogger<ParamSettingServices> ilogger)
        {
            this.unitOfWork = unitOfWork;
            this.ilogger = ilogger;
            _GetAll = default;
        }
        public async Task<IEnumerable<ParamSetting>> GetAllAsync()
        {
            ilogger.LogInformation($"GetAllAsync");
            if (_GetAll == default)
            {
                _GetAll = await unitOfWork.paramSettingRepository.GetAllAsync();
            }
            return _GetAll;
        }
        public async Task<ParamSetting> GetByIdAsync(long Id)
        {
            try
            {
                var a = await unitOfWork.paramSettingRepository.GetByIdAsync(Id);
                ilogger.LogInformation($"Get by id {Id.ToString()} Is {a.ParamKey}");
                return a;
            }
            catch (Exception ex)
            {
                ilogger.LogError($"Get by id {Id.ToString()} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<BaseEntityList<ParamSetting>> GetListAsync(
            Expression<Func<ParamSetting, bool>> expression,
            Func<ParamSetting, object> sort, bool desc,
            int page, int pageSize)
        {
            try
            {
                var a = await unitOfWork.paramSettingRepository.GetListAsync(expression, sort, desc, page, pageSize);
                ilogger.LogInformation($"GetListAsync expression, sort {desc} {page} {pageSize}");
                return a;
            }
            catch (Exception ex)
            {
                ilogger.LogError($"GetListAsync expression, sort {desc} {page} {pageSize} Is Fail {ex.Message}");
                return default;
            }
        }

    }
}
