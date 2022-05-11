using System.Threading.Tasks;
using Utils.Models;

namespace Utils.Repository.Interfaces
{
    public interface IInvoiceRepository
    {
        public CompanyConfig companyConfig { get; set; }
        Task<InvoiceResult> GetInvoice(InvoiceInput inv, int Company = 0);
        Task<InvoiceAllResult> GetInvoiceAll(InvoiceAllInput inv, int Company = 0);
        Task<PayResult> PayInvoice(PayInput inv, int Company = 0);
        Task<PayResult> CheckPayInvoice(CheckPayInput inv, int Company = 0);
        Task<UndoPayResult> UndoPayInvoice(InvoiceInput inv, int Company = 0);
    }
}
