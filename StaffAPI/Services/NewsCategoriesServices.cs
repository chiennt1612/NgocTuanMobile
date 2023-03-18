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
    public class NewsCategoriesServices : INewsCategoriesServices
    {
        private IUnitOfWork unitOfWork;
        private ILogger<NewsCategoriesServices> ilogger;
        public NewsCategoriesServices(IUnitOfWork unitOfWork, ILogger<NewsCategoriesServices> ilogger)
        {
            this.unitOfWork = unitOfWork;
            this.ilogger = ilogger;
        }

        public async Task<IEnumerable<NewsCategories>> GetAllAsync()
        {
            ilogger.LogInformation($"GetAllAsync");
            return await unitOfWork.newsCategoriesRepository.GetAllAsync();
        }

        public async Task<NewsCategories> GetByIdAsync(long Id)
        {
            try
            {
                var a = await unitOfWork.newsCategoriesRepository.GetByIdAsync(Id);
                ilogger.LogInformation($"Get by id {Id.ToString()} Is {a.Name}");
                return a;
            }
            catch (Exception ex)
            {
                ilogger.LogError($"Get by id {Id.ToString()} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<BaseEntityList<NewsCategories>> GetListAsync(
            Expression<Func<NewsCategories, bool>> expression,
            Func<NewsCategories, object> sort, bool desc,
            int page, int pageSize)
        {
            try
            {
                var a = await unitOfWork.newsCategoriesRepository.GetListAsync(expression, sort, desc, page, pageSize);
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
