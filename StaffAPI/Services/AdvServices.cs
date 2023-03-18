using StaffAPI.Repository.Interfaces;
using StaffAPI.Services.Interfaces;
using EntityFramework.API.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StaffAPI.Services
{
    public class AdvServices : IAdvServices
    {
        private IUnitOfWork unitOfWork;
        private ILogger<AdvServices> ilogger;

        public static IEnumerable<Adv> _GetAll; // cache tạm thời

        public AdvServices(IUnitOfWork unitOfWork, ILogger<AdvServices> ilogger)
        {
            this.unitOfWork = unitOfWork;
            this.ilogger = ilogger;
            _GetAll = default;
        }

        public async Task<IEnumerable<Adv>> GetAllAsync()
        {
            ilogger.LogInformation($"GetAllAsync");
            if (_GetAll == default)
            {
                _GetAll = await unitOfWork.advRepository.GetAllAsync();
            }
            return _GetAll;
        }

        public async Task<Adv> GetByIdAsync(long Id)
        {
            try
            {
                var a = await unitOfWork.advRepository.GetByIdAsync(Id);
                try
                {
                    ilogger.LogInformation($"Get by id {Id.ToString()} Is {a.CustomerName}");
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

        public async Task<IEnumerable<Adv>> GetManyAsync(Expression<Func<Adv, bool>> where)
        {
            ilogger.LogInformation($"GetManyAsync");
            return await unitOfWork.advRepository.GetManyAsync(where);
        }
    }
}
