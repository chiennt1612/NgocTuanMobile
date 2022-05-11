using EntityFramework.API.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Auth.Services
{
    public class AppUserManager : UserManager<AppUser>
    {
        private readonly IConfiguration _configuration;
        public AppUserManager(IUserStore<AppUser> store, IOptions<IdentityOptions> optionAccessor,
            IPasswordHasher<AppUser> passwordHasher, IEnumerable<IUserValidator<AppUser>> userValidators,
            IEnumerable<IPasswordValidator<AppUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<AppUser>> logger,
            IConfiguration configuration)
            : base(store, optionAccessor, passwordHasher, userValidators, passwordValidators,
                  keyNormalizer, errors, services, logger)
        {
            _configuration = configuration;
        }
    }
}
