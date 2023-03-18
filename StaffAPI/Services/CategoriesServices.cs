using StaffAPI.Repository.Interfaces;
using StaffAPI.Services.Interfaces;
using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StaffAPI.Services
{
    public class CategoriesServices : ICategoriesServices
    {
        private IUnitOfWork unitOfWork;
        private ILogger<CategoriesServices> ilogger;
        public static IEnumerable<Categories> _GetAll; // cache tạm thời
        public CategoriesServices(IUnitOfWork unitOfWork, ILogger<CategoriesServices> ilogger)
        {
            this.unitOfWork = unitOfWork;
            this.ilogger = ilogger;
            _GetAll = default;
        }

        public async Task<IEnumerable<Categories>> GetAllAsync()
        {
            ilogger.LogInformation($"GetAllAsync");
            if (_GetAll == default)
            {
                _GetAll = await unitOfWork.categoriesRepository.GetAllAsync();
            }
            return _GetAll;
        }

        public async Task<Categories> GetByIdAsync(long Id)
        {
            try
            {
                var a = await unitOfWork.categoriesRepository.GetByIdAsync(Id);
                ilogger.LogInformation($"Get by id {Id.ToString()} Is {a.Name}");
                return a;
            }
            catch (Exception ex)
            {
                ilogger.LogError($"Get by id {Id.ToString()} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<BaseEntityList<Categories>> GetListAsync(
            Expression<Func<Categories, bool>> expression,
            Func<Categories, object> sort, bool desc,
            int page, int pageSize)
        {
            try
            {
                var a = await unitOfWork.categoriesRepository.GetListAsync(expression, sort, desc, page, pageSize);
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
