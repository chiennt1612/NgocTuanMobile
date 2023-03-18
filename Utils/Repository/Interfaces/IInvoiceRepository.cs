using System.Threading.Tasks;
using Utils.Models;

namespace Utils.Repository.Interfaces
{
    public interface IInvoiceRepository
    {
        public CompanyConfig companyConfig { get; set; }

        #region Invoice
        //Invoice "getInvoiceList", "payInvoice", "checkPayInvocie", "undoPayInvocie", "", "", "", "", "", "getInvoiceByQRCode"
        Task<InvoiceResult> GetInvoice(InvoiceInput inv);           //Index: 0
        Task<InvoiceQRCode> getInvoiceByQRCode(InvQrCodeInput inv); //Index: 9
        Task<PayResult> PayInvoice(PayInput inv);                   //Index: 1
        Task<PayResult> CheckPayInvoice(CheckPayInput inv);         //Index: 2
        Task<UndoPayResult> UndoPayInvoice(InvoiceInput inv);       //Index: 3
        #endregion

        #region Customer
        // Customer "", "", "", "", "getInvoiceAllList", "getContractAllList", "getInvoiceAllListA", "getInvoiceListA", "getCustomerInfo", "getInvoiceByQRCode"
        Task<ContractResult> GetContract(ContractInput inv);        //Index: 5
        Task<InvoiceAllResult> GetInvoiceAll(InvoiceAllInput inv);  //Index: 4
        Task<InvoiceDataResult> GetInvoiceA(InvoiceInput inv);      //Index: 7
        Task<InvoiceDataResult> GetInvoiceAllA(InvoiceAllAInput inv);//Index: 6
        Task<CustomerInfoResult> getCustomerInfo(EVNCodeInput inv); //Index: 8
        #endregion

        #region Staff
        // Staff "getInvoiceList", "payInvoice", "checkPayInvocie", "undoPayInvocie", "getStaffInfo", "", "", "", "", "getInvoiceByQRCode"
        Task<StaffInfoResult> getStaffInfo(StaffCodeInput inv);     //Index: 4
        #endregion
    }
}
