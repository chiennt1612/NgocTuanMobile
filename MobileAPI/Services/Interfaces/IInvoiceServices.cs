using System.Threading.Tasks;
using Utils.Models;

namespace MobileAPI.Services.Interfaces
{
    public interface IInvoiceServices
    {
        Task<ResponseOK> GetInvoice(InvoiceInput inv);
        Task<ResponseOK> GetInvoiceAll(InvoiceAllInput inv);
        Task<ResponseOK> PayInvoice(PayInput inv);
        Task<ResponseOK> CheckPayInvoice(CheckPayInput inv);
        Task<ResponseOK> UndoPayInvoice(InvoiceInput inv);
        Task<InvoiceResult> CheckInvoice(InvoiceInput inv);
        Task<InvoiceAllResult> CheckAllInvoice(InvoiceAllInput inv);
        Task<InvoiceDataResult> GetInvoiceA(InvoiceInput inv);
        Task<InvoiceDataResult> GetInvoiceAllA(InvoiceAllAInput inv);
        Task<CustomerInfoResult> getCustomerInfo(EVNCodeInput inv);
        Task<InvoiceQRCode> getInvoiceByQRCode(InvQrCodeInput inv);
    }
}
