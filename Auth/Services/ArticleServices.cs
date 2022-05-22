using Auth.Repository.Interfaces;
using Auth.Services.Interfaces;
using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Auth.Services
{
    public class ArticleServices : IArticleServices
    {
        private IUnitOfWork unitOfWork;
        private ILogger<ArticleServices> ilogger;
        public ArticleServices(IUnitOfWork unitOfWork, ILogger<ArticleServices> ilogger)
        {
            this.unitOfWork = unitOfWork;
            this.ilogger = ilogger;
        }

        public async Task<IEnumerable<NewsCategories>> CateGetAllAsync()
        {
            ilogger.LogInformation($"GetAllAsync");
            return await unitOfWork.articleRepository.CateGetAllAsync();
        }
        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            ilogger.LogInformation($"GetAllAsync");
            return await unitOfWork.articleRepository.GetAllAsync();
        }
        public async Task<Article> GetByIdAsync(long Id)
        {
            try
            {
                var a = await unitOfWork.articleRepository.GetByIdAsync(Id);
                try
                {
                    ilogger.LogInformation($"Get by id {Id.ToString()} Is {JsonConvert.SerializeObject(a)}");
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

        public async Task<BaseEntityList<Article>> GetListAsync(
            Expression<Func<Article, bool>> expression,
            Func<Article, object> sort, bool desc,
            int page, int pageSize)
        {
            try
            {
                var a = await unitOfWork.articleRepository.GetListAsync(expression, sort, desc, page, pageSize);
                ilogger.LogInformation($"GetListAsync expression, sort {desc} {page} {pageSize}");
                return a;
            }
            catch (Exception ex)
            {
                ilogger.LogError($"GetListAsync expression, sort {desc} {page} {pageSize} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<IList<Article>> GetTopAsync(Expression<Func<Article, bool>> expression, int top)
        {
            return await unitOfWork.articleRepository.GetTopAsync(expression, top);
        }
    }
}
