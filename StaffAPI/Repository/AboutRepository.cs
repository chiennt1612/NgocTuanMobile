using StaffAPI.Repository.Interfaces;
using EntityFramework.API.DBContext;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StaffAPI.Repository
{
    public class AboutRepository : GenericRepository<About, long>, IAboutRepository
    {
        private readonly AppDbContext _context;
        public AboutRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
            : base(dbContext, contextAccessor)
        {
            _context = dbContext;
        }

        public override async Task<About> GetByIdAsync(long id)
        {
            var p1 = new SqlParameter("@Id", id);
            var author = await _context.Abouts.FromSqlRaw($"SELECT * From About Where Id = @Id", p1).FirstOrDefaultAsync();
            return author;
            //return await _context.Articles
            //    .Include(a => a.MainCategories)
            //    .Include(a => a.ReferCategories)
            //    .FirstOrDefaultAsync(m => m.Id == id);
        }

        public override async Task<IEnumerable<About>> GetAllAsync()
        {
            await Task.Delay(0);
            var a = _context.Abouts.FromSqlRaw($"SELECT * From About");
            return a;
        }

        public async Task<IEnumerable<About>> GetListAsync(IEnumerable<long> Ids)
        {
            await Task.Delay(0);
            var a = _context.Abouts.FromSqlRaw($"SELECT * From About Where Id in ({string.Join(", ", Ids)})");
            return a;
        }
    }
}
