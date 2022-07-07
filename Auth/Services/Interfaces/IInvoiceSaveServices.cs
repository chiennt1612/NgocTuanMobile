using Auth.Models;
using EntityFramework.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Services.Interfaces
{
    public interface IInvoiceSaveServices
    {
        Task<InvoiceSave> AddAsync(InvoiceSave contact);
        Task<InvoiceSave> GetByIdAsync(long Id);
        Task<IList<InvoiceSave>> InvoceSaveGetListAsync(int? Page, int? PageSize, SearchDateModel exp);
    }
}
