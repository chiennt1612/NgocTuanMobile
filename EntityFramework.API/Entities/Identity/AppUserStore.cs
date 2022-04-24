using EntityFramework.API.DBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EntityFramework.API.Entities.Identity
{
    public class AppUserStore : UserStore<AppUser, AppRole, UserDbContext, long>
    {
        public AppUserStore(UserDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
        }
    }
}
