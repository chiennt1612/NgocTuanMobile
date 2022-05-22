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
    public class CategoriesRepository : GenericRepository<Categories, long>, ICategoriesRepository
    {
        private readonly AppDbContext _context;
        public CategoriesRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
              : base(dbContext, contextAccessor)
        {
            _context = dbContext;
        }

        public override async Task<IEnumerable<Categories>> GetAllAsync()
        {
            return await _context.Categoriess.Include(a => a.Parent).Where(a => a.IsDeleted == false).ToListAsync();
        }
        public override async Task<Categories> GetByIdAsync(long id)
        {
            return await _context.Categoriess.Include(a => a.Parent).FirstOrDefaultAsync(m => m.Id == id);
        }

        public override async Task<BaseEntityList<Categories>> GetListAsync(
            Expression<Func<Categories, bool>> expression,
            Func<Categories, object> sort, bool desc,
            int page, int pageSize)
        {
            var a = new BaseEntityList<Categories>();
            a.totalRecords = await CountAsync(expression);
            int skipRows = (page - 1) * pageSize;
            if (desc)
                a.list = (await _context.Categoriess
                            .Include(a => a.Parent)
                            .Where(a => a.IsDeleted == false)
                            .Where(expression).ToListAsync()
                          ).OrderByDescending(sort).Skip(skipRows).Take(pageSize);
            else
                a.list = (await _context.Categoriess
                            .Include(a => a.Parent)
                            .Where(a => a.IsDeleted == false)
                            .Where(expression).ToListAsync()
                          ).OrderBy(sort).Skip(skipRows).Take(pageSize);
            return a;
        }
    }
}
