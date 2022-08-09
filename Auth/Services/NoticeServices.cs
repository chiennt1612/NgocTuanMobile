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
    public class NoticeServices : INoticeServices
    {
        private IUnitOfWork unitOfWork;
        private ILogger<NoticeServices> ilogger;

        public NoticeServices(IUnitOfWork unitOfWork, ILogger<NoticeServices> ilogger)
        {
            this.unitOfWork = unitOfWork;
            this.ilogger = ilogger;
        }

        public async Task<IEnumerable<Notice>> GetAllAsync()
        {
            if (ilogger != null) ilogger.LogInformation($"GetAllAsync");
            return await unitOfWork.noticeRepository.GetAllAsync();
        }

        public async Task<Notice> GetByIdAsync(long Id)
        {
            try
            {
                var a = await unitOfWork.noticeRepository.GetByIdAsync(Id);
                return a;
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"Get by id {Id.ToString()} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<BaseEntityList<Notice>> GetListAsync(
            Expression<Func<Notice, bool>> expression,
            Func<Notice, object> sort, bool desc,
            int page, int pageSize)
        {
            try
            {
                var a = await unitOfWork.noticeRepository.GetListAsync(expression, sort, desc, page, pageSize);
                if (ilogger != null) ilogger.LogInformation($"GetListAsync expression, sort {desc} {page} {pageSize}");
                return a;
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"GetListAsync expression, sort {desc} {page} {pageSize} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<Notice> AddAsync(Notice notice)
        {
            var a = await unitOfWork.noticeRepository.AddAsync(notice);
            await unitOfWork.SaveAsync();
            return a;
        }

        public async Task AddManyAsync(IEnumerable<Notice> notice)
        {
            await unitOfWork.noticeRepository.AddManyAsync(notice);
            await unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(long id)
        {
            await unitOfWork.noticeRepository.DeleteAsync(id);
            await unitOfWork.SaveAsync();
        }
    }
}
