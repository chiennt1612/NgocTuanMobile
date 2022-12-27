using MobileAPI.Repository.Interfaces;
using EntityFramework.API.DBContext;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileAPI.Repository
{
    public class ServiceRepository : GenericRepository<Service, long>, IServiceRepository
    {
        private readonly AppDbContext _context;
        public ServiceRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
            : base(dbContext, contextAccessor)
        {
            _context = dbContext;
        }

        public override async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _context.Services
                            .Where(a => a.IsDeleted == false).ToListAsync();
        }

        public override async Task<Service> GetByIdAsync(long id)
        {
            return await _context.Services
                            .Where(a => a.IsDeleted == false)
                            .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
