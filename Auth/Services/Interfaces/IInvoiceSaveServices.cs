using Auth.Models;
using EntityFramework.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auth.Services.Interfaces
{
    public interface IInvoiceSaveServices
    {
        Task<bool> DeleteAsync(long id);
        Task<InvoiceSave> AddAsync(InvoiceSave contact);
        Task<InvoiceSave> GetByIdAsync(long Id);
        Task<IList<InvoiceSave>> InvoceSaveGetListAsync(int? Page, int? PageSize, SearchDateModel exp);
    }
}
