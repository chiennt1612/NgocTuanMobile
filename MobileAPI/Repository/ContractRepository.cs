using MobileAPI.Repository.Interfaces;
using EntityFramework.API.DBContext;
using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MobileAPI.Repository
{
    public class ContractRepository : GenericRepository<Contract, long>, IContractRepository
    {
        private readonly AppDbContext _context;
        public ContractRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
            : base(dbContext, contextAccessor)
        {
            _context = dbContext;
        }

        public override async Task<BaseEntityList<Contract>> GetListAsync(
            Expression<Func<Contract, bool>> expression,
            Func<Contract, object> sort, bool desc,
            int page, int pageSize)
        {
            var a = new BaseEntityList<Contract>();
            a.totalRecords = await CountAsync(expression);
            a.page = page;
            a.pageSize = pageSize;
            int skipRows = (page - 1) * pageSize;
            if (desc)
                a.list = (await _context.Contracts
                            .Where(expression).ToListAsync()
                          ).OrderByDescending(sort).Skip(skipRows).Take(pageSize);
            else
                a.list = (await _context.Contracts
                            .Where(expression).ToListAsync()
                          ).OrderBy(sort).Skip(skipRows).Take(pageSize);
            return a;
        }
    }
}
