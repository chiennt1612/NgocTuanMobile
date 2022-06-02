using Auth.Repository.Interfaces;
using EntityFramework.API.DBContext;
using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Auth.Repository
{
    public class ArticleRepository : GenericRepository<Article, long>, IArticleRepository
    {
        private readonly AppDbContext _context;
        public ArticleRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
            : base(dbContext, contextAccessor)
        {
            _context = dbContext;
        }
        public async Task<IEnumerable<NewsCategories>> CateGetAllAsync()
        {
            return await _context.NewsCategoriess.Include(a => a.Parent).Where(a => a.IsDeleted == false).ToListAsync();
        }
        public async Task<IList<Article>> GetTopAsync(Expression<Func<Article, bool>> expression, int top)
        {
            return await _context.Articles
                            .Include(a => a.MainCategories)
                            .Include(a => a.ReferCategories)
                            .Where(a => a.IsDeleted == false)
                            .Where(expression)
                            .OrderByDescending(u => u.Id).Take(top)
                            .ToListAsync();
        }
        public override async Task<Article> GetByIdAsync(long id)
        {
            return await _context.Articles
                .Include(a => a.MainCategories)
                .Include(a => a.ReferCategories)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public override async Task<BaseEntityList<Article>> GetListAsync(
            Expression<Func<Article, bool>> expression,
            Func<Article, object> sort, bool desc,
            int page, int pageSize)
        {
            var a = new BaseEntityList<Article>();
            a.totalRecords = await CountAsync(expression);
            a.page = page;
            a.pageSize = pageSize;
            int skipRows = (page - 1) * pageSize;
            if (desc)
                a.list = (await _context.Articles
                            .Include(a => a.MainCategories)
                            .Include(a => a.ReferCategories)
                            .Where(a => a.IsDeleted == false)
                            .Where(expression).ToListAsync()
                          ).OrderByDescending(sort).Skip(skipRows).Take(pageSize);
            else
                a.list = (await _context.Articles
                            .Include(a => a.MainCategories)
                            .Include(a => a.ReferCategories)
                            .Where(a => a.IsDeleted == false)
                            .Where(expression).ToListAsync()
                          ).OrderBy(sort).Skip(skipRows).Take(pageSize);
            return a;
        }
    }
}
