using MobileAPI.Repository.Interfaces;
using MobileAPI.Services.Interfaces;
using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MobileAPI.Services
{
    public class AboutServices : IAboutServices
    {
        private IUnitOfWork unitOfWork;
        private ILogger<AboutServices> ilogger;

        public static IEnumerable<About> _GetAll; // cache tạm thời

        public AboutServices(IUnitOfWork unitOfWork, ILogger<AboutServices> ilogger)
        {
            this.unitOfWork = unitOfWork;
            this.ilogger = ilogger;
            _GetAll = default;
        }

        public async Task<IEnumerable<About>> GetManyAsync(Expression<Func<About, bool>> where)
        {
            if (ilogger != null) ilogger.LogInformation($"GetManyAsync");

            return await unitOfWork.aboutRepository.GetManyAsync(where); ;
        }

        public async Task<IEnumerable<About>> GetAllAsync()
        {
            ilogger.LogInformation($"GetAllAsync");
            if (_GetAll == default)
            {
                _GetAll = await unitOfWork.aboutRepository.GetAllAsync();
            }
            return _GetAll;
        }

        public async Task<About> GetByIdAsync(long Id)
        {
            try
            {
                var a = await unitOfWork.aboutRepository.GetByIdAsync(Id);
                try
                {
                    ilogger.LogInformation($"Get by id {Id.ToString()} Is {a.Title}");
                }
                catch (Exception ex)
                {
                    ilogger.LogInformation($"Get by id {Id.ToString()} Is {ex.Message}");
                }
                return a;
            }
            catch (Exception ex)
            {
                ilogger.LogError($"Get by id {Id.ToString()} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<BaseEntityList<About>> GetListAsync(
            Expression<Func<About, bool>> expression,
            Func<About, object> sort, bool desc,
            int page, int pageSize)
        {
            try
            {
                var a = await unitOfWork.aboutRepository.GetListAsync(expression, sort, desc, page, pageSize);
                ilogger.LogInformation($"GetListAsync expression, sort {desc} {page} {pageSize}");
                return a;
            }
            catch (Exception ex)
            {
                ilogger.LogError($"GetListAsync expression, sort {desc} {page} {pageSize} Is Fail {ex.Message}");
                return default;
            }
        }
        public async Task<IEnumerable<About>> GetListAsync(IEnumerable<long> Ids)
        {
            var a = await unitOfWork.aboutRepository.GetListAsync(Ids);
            return a;
        }
    }
}
