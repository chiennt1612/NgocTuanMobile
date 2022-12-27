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
    public class FAQServices : IFAQServices
    {
        private IUnitOfWork unitOfWork;
        private ILogger<FAQServices> ilogger;

        public static IEnumerable<FAQ> _GetAll; // cache tạm thời

        public FAQServices(IUnitOfWork unitOfWork, ILogger<FAQServices> ilogger)
        {
            this.unitOfWork = unitOfWork;
            this.ilogger = ilogger;
            _GetAll = default;
        }

        public async Task<IEnumerable<FAQ>> GetAllAsync()
        {
            ilogger.LogInformation($"GetAllAsync");
            if (_GetAll == default)
            {
                _GetAll = await unitOfWork.fAQRepository.GetAllAsync();
            }
            return _GetAll;
        }

        public async Task<FAQ> GetByIdAsync(long Id)
        {
            try
            {
                var a = await unitOfWork.fAQRepository.GetByIdAsync(Id);
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

        public async Task<BaseEntityList<FAQ>> GetListAsync(
            Expression<Func<FAQ, bool>> expression,
            Func<FAQ, object> sort, bool desc,
            int page, int pageSize)
        {
            try
            {
                var a = await unitOfWork.fAQRepository.GetListAsync(expression, sort, desc, page, pageSize);
                ilogger.LogInformation($"GetListAsync expression, sort {desc} {page} {pageSize}");
                return a;
            }
            catch (Exception ex)
            {
                ilogger.LogError($"GetListAsync expression, sort {desc} {page} {pageSize} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<IList<FAQ>> GetTopAsync(Expression<Func<FAQ, bool>> expression, int top)
        {
            return await unitOfWork.fAQRepository.GetTopAsync(expression, top);
        }
    }
}
