﻿using EntityFramework.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils.Models;

namespace Auth.Services.Interfaces
{
    public interface IInvoiceSaveServices
    {
        Task<bool> DeleteAsync(long id);
        Task<InvoiceSave> AddAsync(InvoiceSave contact);
        Task<InvoiceSave> GetByIdAsync(long Id);
        Task<IList<InvoiceSave>> InvoceSaveGetListAsync(int? Page, int? PageSize, InvoiceSaveSearchModelA exp);
        Task<int> GetCountAsync(InvoiceSaveSearchModelA exp);
    }
}
