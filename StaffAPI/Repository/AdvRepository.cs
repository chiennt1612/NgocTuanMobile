using EntityFramework.API.DBContext;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StaffAPI.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffAPI.Repository
{
    public class AdvRepository : GenericRepository<Adv, long>, IAdvRepository
    {
        private readonly AppDbContext _context;
        public AdvRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
            : base(dbContext, contextAccessor)
        {
            _context = dbContext;
        }

        public override async Task<IEnumerable<Adv>> GetAllAsync()
        {
            var d = DateTime.Now;
            return await _context.Advs.Include(a => a.AdvPosition).Where(
                a => a.IsDeleted == false &&
                a.Status &&
                a.StartDate <= d &&
                (a.EndDate.HasValue ? a.EndDate.Value >= d : true)).ToListAsync();
        }
    }
}
