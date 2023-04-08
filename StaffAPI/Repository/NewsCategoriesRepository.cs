using EntityFramework.API.DBContext;
using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StaffAPI.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StaffAPI.Repository
{
    public class NewsCategoriesRepository : GenericRepository<NewsCategories, long>, INewsCategoriesRepository
    {
        private readonly AppDbContext _context;
        public NewsCategoriesRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
              : base(dbContext, contextAccessor)
        {
            _context = dbContext;
        }

        public override async Task<IEnumerable<NewsCategories>> GetAllAsync()
        {
            return await _context.NewsCategoriess.Include(a => a.Parent).Where(a => a.IsDeleted == false).ToListAsync();
        }
        public override async Task<NewsCategories> GetByIdAsync(long id)
        {
            return await _context.NewsCategoriess.Include(a => a.Parent).FirstOrDefaultAsync(m => m.Id == id);
        }

        public override async Task<BaseEntityList<NewsCategories>> GetListAsync(
            Expression<Func<NewsCategories, bool>> expression,
            Func<NewsCategories, object> sort, bool desc,
            int page, int pageSize)
        {
            var a = new BaseEntityList<NewsCategories>();
            a.totalRecords = await CountAsync(expression);
            int skipRows = (page - 1) * pageSize;
            if (desc)
                a.list = (await _context.NewsCategoriess
                            .Include(a => a.Parent)
                            .Where(a => a.IsDeleted == false)
                            .Where(expression).ToListAsync()
                          ).OrderByDescending(sort).Skip(skipRows).Take(pageSize);
            else
                a.list = (await _context.NewsCategoriess
                            .Include(a => a.Parent)
                            .Where(a => a.IsDeleted == false)
                            .Where(expression).ToListAsync()
                          ).OrderBy(sort).Skip(skipRows).Take(pageSize);
            return a;
        }
    }
}
