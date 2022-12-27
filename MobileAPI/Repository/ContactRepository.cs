using MobileAPI.Repository.Interfaces;
using EntityFramework.API.DBContext;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Http;

namespace MobileAPI.Repository
{
    public class ContactRepository : GenericRepository<Contact, long>, IContactRepository
    {
        private readonly AppDbContext _context;
        public ContactRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
            : base(dbContext, contextAccessor)
        {
            _context = dbContext;
        }
    }
}
