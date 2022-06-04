using System.Threading.Tasks;
using Utils.Models;

namespace Auth.Services.Interfaces
{
    public interface IInvoiceServices
    {
        Task<ResponseOK> GetInvoice(InvoiceInput inv);
        Task<ResponseOK> GetInvoiceAll(InvoiceAllInput inv);
        Task<ResponseOK> PayInvoice(PayInput inv);
        Task<ResponseOK> CheckPayInvoice(CheckPayInput inv);
        Task<ResponseOK> UndoPayInvoice(InvoiceInput inv);
    }
}
