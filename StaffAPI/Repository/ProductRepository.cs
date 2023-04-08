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
    public class ProductRepository : GenericRepository<Product, long>, IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
              : base(dbContext, contextAccessor)
        {
            _context = dbContext;
        }
        public async Task<IEnumerable<Categories>> CateGetAllAsync()
        {
            return await _context.Categoriess.Include(a => a.Parent).Where(a => a.IsDeleted == false).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetTopAsync(Expression<Func<Product, bool>> expression, int top)
        {
            return (await _context.Products
                            .Include(a => a.MainCategories)
                            .Include(a => a.ReferCategories)
                            .Where(a => a.IsDeleted == false)
                            .Where(expression).ToListAsync()
                          ).OrderByDescending(u => u.Id).Take(top);
        }
        public override async Task<Product> GetByIdAsync(long id)
        {
            return await _context.Products
                .Include(a => a.MainCategories)
                .Include(a => a.ReferCategories)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public override async Task<BaseEntityList<Product>> GetListAsync(
            Expression<Func<Product, bool>> expression,
            Func<Product, object> sort, bool desc,
            int page, int pageSize)
        {
            var a = new BaseEntityList<Product>();
            a.totalRecords = await CountAsync(expression);
            int skipRows = (page - 1) * pageSize;
            if (desc)
                a.list = (await _context.Products
                            .Include(a => a.MainCategories)
                            .Include(a => a.ReferCategories)
                            .Where(a => a.IsDeleted == false)
                            .Where(expression).ToListAsync()
                          ).OrderByDescending(sort).Skip(skipRows).Take(pageSize);
            else
                a.list = (await _context.Products
                            .Include(a => a.MainCategories)
                            .Include(a => a.ReferCategories)
                            .Where(a => a.IsDeleted == false)
                            .Where(expression).ToListAsync()
                          ).OrderBy(sort).Skip(skipRows).Take(pageSize);
            return a;
        }
    }
}
