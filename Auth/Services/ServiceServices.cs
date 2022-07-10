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

        public static IEnumerable<Service> _GetAll; // cache tạm thời

        public ServiceServices(IUnitOfWork unitOfWork, ILogger<ServiceServices> ilogger)
        {
            this.unitOfWork = unitOfWork;
            this.ilogger = ilogger;
            _GetAll = default;
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            ilogger.LogInformation($"GetAllAsync");
            if (_GetAll == default)
            {
                _GetAll = await unitOfWork.serviceRepository.GetAllAsync();
            }
            return _GetAll;
        }

        public async Task<Service> GetByIdAsync(long Id)
        {
            try
            {
                var a = await unitOfWork.serviceRepository.GetByIdAsync(Id);
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

        public async Task<BaseEntityList<Service>> GetListAsync(
            Expression<Func<Service, bool>> expression,
            Func<Service, object> sort, bool desc,
            int page, int pageSize)
        {
            try
            {
                var a = await unitOfWork.serviceRepository.GetListAsync(expression, sort, desc, page, pageSize);
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
