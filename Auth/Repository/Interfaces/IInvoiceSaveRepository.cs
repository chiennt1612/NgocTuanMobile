﻿using Auth.Models;
using EntityFramework.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auth.Repository.Interfaces
{
    public interface IInvoiceSaveRepository : IGenericRepository<InvoiceSave, long>
    {
        Task<IList<InvoiceSave>> InvoceSaveGetListAsync(int? Page, int? PageSize, SearchDateModel expression);
    }
}
