﻿using EntityFramework.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StaffAPI.Services.Interfaces
{
    public interface IAdvServices
    {
        Task<IEnumerable<Adv>> GetAllAsync();
        Task<Adv> GetByIdAsync(long Id);
        Task<IEnumerable<Adv>> GetManyAsync(Expression<Func<Adv, bool>> where);
    }
}
