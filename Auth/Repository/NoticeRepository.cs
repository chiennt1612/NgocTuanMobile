using Auth.Repository.Interfaces;
using EntityFramework.API.DBContext;
using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Auth.Repository
{
    public class NoticeRepository : GenericRepository<Notice, long>, INoticeRepository
    {
        private readonly AppDbContext _context;
        public NoticeRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
            : base(dbContext, contextAccessor)
        {
            _context = dbContext;
        }

        public override async Task<BaseEntityList<Notice>> GetListAsync(
            Expression<Func<Notice, bool>> expression,
            Func<Notice, object> sort, bool desc,
            int page, int pageSize)
        {
            var a = new BaseEntityList<Notice>();
            a.totalRecords = await CountAsync(expression);
            a.page = page;
            a.pageSize = pageSize;
            int skipRows = (page - 1) * pageSize;
            if (desc)
                a.list = (await _context.Notices
                            .Where(a => a.IsDelete == false)
                            .Where(expression).ToListAsync()
                          ).OrderByDescending(sort).Skip(skipRows).Take(pageSize);
            else
                a.list = (await _context.Notices
                            .Where(a => a.IsDelete == false)
                            .Where(expression).ToListAsync()
                          ).OrderBy(sort).Skip(skipRows).Take(pageSize);
            return a;
        }
    }
}
