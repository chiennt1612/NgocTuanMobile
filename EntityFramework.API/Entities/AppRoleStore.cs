using EntityFramework.API.DBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EntityFramework.API.Entities
{
    public class AppRoleStore : RoleStore<AppRole, UserDbContext, long>
    {
        public AppRoleStore(UserDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
        }
    }
}
