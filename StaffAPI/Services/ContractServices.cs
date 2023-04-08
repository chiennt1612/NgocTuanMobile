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
    public class ContractServices : IContractServices
    {
        private IUnitOfWork unitOfWork;
        private ILogger<ContractServices> ilogger;

        public ContractServices(IUnitOfWork unitOfWork, ILogger<ContractServices> ilogger)
        {
            this.unitOfWork = unitOfWork;
            this.ilogger = ilogger;
        }

        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            if (ilogger != null) ilogger.LogInformation($"GetAllAsync");
            return await unitOfWork.contractRepository.GetAllAsync();
        }

        public async Task<Contract> GetByIdAsync(long Id)
        {
            try
            {
                var a = await unitOfWork.contractRepository.GetByIdAsync(Id);
                return a;
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"Get by id {Id.ToString()} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<BaseEntityList<Contract>> GetListAsync(
            Expression<Func<Contract, bool>> expression,
            Func<Contract, object> sort, bool desc,
            int page, int pageSize)
        {
            try
            {
                var a = await unitOfWork.contractRepository.GetListAsync(expression, sort, desc, page, pageSize);
                if (ilogger != null) ilogger.LogInformation($"GetListAsync expression, sort {desc} {page} {pageSize}");
                return a;
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"GetListAsync expression, sort {desc} {page} {pageSize} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<Contract> AddAsync(Contract contract)
        {
            var a = await unitOfWork.contractRepository.AddAsync(contract);
            await unitOfWork.SaveAsync();
            return a;
        }

        public async Task AddManyAsync(IEnumerable<Contract> contract)
        {
            await unitOfWork.contractRepository.AddManyAsync(contract);
            await unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(long id)
        {
            await unitOfWork.contractRepository.DeleteAsync(id);
            await unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(Contract notice)
        {
            unitOfWork.contractRepository.Delete(notice);
            await unitOfWork.SaveAsync();
        }

        public async Task<Contract> GetAsync(Expression<Func<Contract, bool>> where)
        {
            var a = await unitOfWork.contractRepository.GetAsync(where);
            return a;
        }

        public async Task<IEnumerable<Contract>> GetManyAsync(Expression<Func<Contract, bool>> where)
        {
            var a = await unitOfWork.contractRepository.GetManyAsync(where);
            return a;
        }
    }
}
