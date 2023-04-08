using Auth.Repository.Interfaces;
using Auth.Services.Interfaces;
using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Auth.Services
{
    public class ServiceServices : IServiceServices
    {
        private IUnitOfWork unitOfWork;
        private ILogger<ServiceServices> ilogger;

        public ServiceServices(IUnitOfWork unitOfWork, ILogger<ServiceServices> ilogger)
        {
            this.unitOfWork = unitOfWork;
            this.ilogger = ilogger;
        }

        public async Task<IEnumerable<Service>> GetManyAsync(Expression<Func<Service, bool>> where)
        {
            if (ilogger != null) ilogger.LogInformation($"GetManyAsync");

            return await unitOfWork.serviceRepository.GetManyAsync(where); ;
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            if (ilogger != null) ilogger.LogInformation($"GetAllAsync");
            return await unitOfWork.serviceRepository.GetAllAsync(); ;
        }

        public async Task<Service> GetByIdAsync(long Id)
        {
            try
            {
                var a = await unitOfWork.serviceRepository.GetByIdAsync(Id);
                try
                {
                    if (ilogger != null) ilogger.LogInformation($"Get by id {Id.ToString()} Is {a.Title}");
                }
                catch (Exception ex)
                {
                    if (ilogger != null) ilogger.LogInformation($"Get by id {Id.ToString()} Is {ex.Message}");
                }
                return a;
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"Get by id {Id.ToString()} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<BaseEntityList<Service>> GetListAsync(
            Expression<Func<Service, bool>> expression,
            Func<Service, object> sort, bool desc,
            int page, int pageSize)
        {
            try
            {
                var a = await unitOfWork.serviceRepository.GetListAsync(expression, sort, desc, page, pageSize);
                if (ilogger != null) ilogger.LogInformation($"GetListAsync expression, sort {desc} {page} {pageSize}");
                return a;
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"GetListAsync expression, sort {desc} {page} {pageSize} Is Fail {ex.Message}");
                return default;
            }
        }

    }
}
