using System.Threading.Tasks;
using Utils.Models;

namespace StaffAPI.Services.Interfaces
{
    public interface IInvoiceServices
    {
        #region Invoice
        Task<ResponseOK> GetInvoice(InvoiceInput inv);
        Task<InvoiceResult> CheckInvoice(InvoiceInput inv);
        Task<InvoiceQRCode> getInvoiceByQRCode(InvQrCodeInput inv);
        Task<ResponseOK> PayInvoice(PayInput inv);
        Task<ResponseOK> CheckPayInvoice(CheckPayInput inv);
        Task<ResponseOK> UndoPayInvoice(CheckPayInput inv);
        #endregion

        #region Customer
        Task<ResponseOK> GetInvoiceAll(InvoiceAllInput inv);
        Task<InvoiceAllResult> CheckAllInvoice(InvoiceAllInput inv);
        Task<InvoiceDataResult> GetInvoiceA(InvoiceInput inv);
        Task<InvoiceDataResult> GetInvoiceAllA(InvoiceAllAInput inv);
        Task<CustomerInfoResult> getCustomerInfo(EVNCodeInput inv);
        #endregion

        #region Staff
        Task<StaffInfoResult> getStaffInfo(StaffCodeInput inv);
        Task<ResponseOK> PayInvoiceByStaff(PayInputByStaff inv);
        Task<ResponseOK> CheckPayInvoiceByStaff(CheckPayInputByStaff inv);
        Task<ResponseOK> UndoPayInvoiceByStaff(CheckPayInputByStaff inv);
        #endregion












    }
}
