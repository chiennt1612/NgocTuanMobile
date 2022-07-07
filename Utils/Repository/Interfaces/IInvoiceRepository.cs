using System.Threading.Tasks;
using Utils.Models;

namespace Utils.Repository.Interfaces
{
    public interface IInvoiceRepository
    {
        public CompanyConfig companyConfig { get; set; }
        Task<ContractResult> GetContract(ContractInput inv);
        Task<InvoiceResult> GetInvoice(InvoiceInput inv);
        Task<InvoiceAllResult> GetInvoiceAll(InvoiceAllInput inv);
        Task<PayResult> PayInvoice(PayInput inv);
        Task<PayResult> CheckPayInvoice(CheckPayInput inv);
        Task<UndoPayResult> UndoPayInvoice(InvoiceInput inv);
        Task<InvoiceDataResult> GetInvoiceA(InvoiceInput inv);
        Task<CustomerInfoResult> getCustomerInfo(EVNCodeInput inv);
        Task<InvoiceQRCode> getInvoiceByQRCode(InvQrCodeInput inv);
    }
}
