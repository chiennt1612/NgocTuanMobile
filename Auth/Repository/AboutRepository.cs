using Auth.Repository.Interfaces;
using EntityFramework.API.DBContext;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Http;

namespace Auth.Repository
{
    public class AboutRepository : GenericRepository<About, long>, IAboutRepository
    {
        private readonly AppDbContext _context;
        public AboutRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
            : base(dbContext, contextAccessor)
        {
            _context = dbContext;
        }
    }
}
