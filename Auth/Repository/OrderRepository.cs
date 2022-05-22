using Auth.Repository.Interfaces;
using EntityFramework.API.DBContext;
using EntityFramework.API.Entities.EntityBase;
using EntityFramework.API.Entities.Ordering;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
//using X.PagedList;

namespace Auth.Repository
{
    public class OrderRepository : GenericRepository<Order, long>, IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly HttpContext _httpContext;
        private readonly DbSet<Order> _dbSet;
        public OrderRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
            : base(dbContext, contextAccessor)
        {
            _context = dbContext;
            _httpContext = contextAccessor.HttpContext;
            _dbSet = _context.Set<Order>();
        }
        public override async Task<Order> GetByIdAsync(long id)
        {
            return await _context.Orders.Include(a => a.OrderStatus).FirstOrDefaultAsync(m => m.Id == id);
        }
        public override async Task<BaseEntityList<Order>> GetListAsync(
            Expression<Func<Order, bool>> expression,
            Func<Order, object> sort, bool desc,
            int page, int pageSize)
        {
            var a = new BaseEntityList<Order>();
            a.totalRecords = await CountAsync(expression);
            int skipRows = (page - 1) * pageSize;
            if (desc)
                a.list = (await _dbSet.AsNoTracking().Include(a => a.OrderStatus).Where(expression).ToListAsync()).OrderByDescending(sort).Skip(skipRows).Take(pageSize).ToList();
            else
                a.list = (await _dbSet.AsNoTracking().Include(a => a.OrderStatus).Where(expression).ToListAsync()).OrderBy(sort).Skip(skipRows).Take(pageSize).ToList();
            return a;
        }
        //public async Task<int> CountAsync(Expression<Func<Order, bool>> expression)
        //{
        //    return await _dbSet.AsNoTracking().CountAsync(expression);
        //}

    }
}
