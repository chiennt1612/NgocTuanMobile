using EntityFramework.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils.Models;

namespace Auth.Repository.Interfaces
{
    public interface IInvoiceSaveRepository : IGenericRepository<InvoiceSave, long>
    {
        Task<IList<InvoiceSave>> InvoceSaveGetListAsync(int? Page, int? PageSize, InvoiceSaveSearchModelA expression);
        Task<int> GetCountAsync(InvoiceSaveSearchModelA expression);
    }
}
