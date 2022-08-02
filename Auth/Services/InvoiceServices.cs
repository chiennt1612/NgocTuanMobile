using Auth.Services.Interfaces;
using System.Threading.Tasks;
using Utils.Models;
using Utils.Repository.Interfaces;

namespace Auth.Services
{
    public class InvoiceServices : IInvoiceServices
    {
        private readonly IInvoiceRepository _invoice;

        public InvoiceServices(IInvoiceRepository _invoice)
        {
            this._invoice = _invoice;
        }
        public async Task<ResponseOK> CheckPayInvoice(CheckPayInput inv)
        {
            var a = await _invoice.CheckPayInvoice(inv);
            if (a.PayStatus == "00")
                return new ResponseOK()
                {
                    Code = 200,
                    data = a,
                    InternalMessage = LanguageAll.Language.Success,
                    MoreInfo = LanguageAll.Language.Success,
                    Status = 1,
                    UserMessage = LanguageAll.Language.Success
                };
            else
                return new ResponseOK()
                {
                    Code = 404,
                    data = a,
                    InternalMessage = LanguageAll.Language.Fail,
                    MoreInfo = LanguageAll.Language.Fail,
                    Status = 0,
                    UserMessage = LanguageAll.Language.Fail
                };
        }

        public async Task<ResponseOK> GetInvoice(InvoiceInput inv)
        {
            var a = await _invoice.GetInvoice(inv);
            if (a.DataStatus == "00")
            {
                a.ItemsData.CompanyID = inv.CompanyID;
                return new ResponseOK()
                {
                    Code = 200,
                    data = a,
                    InternalMessage = LanguageAll.Language.Success,
                    MoreInfo = LanguageAll.Language.Success,
                    Status = 1,
                    UserMessage = LanguageAll.Language.Success
                };
            }
            else
                return new ResponseOK()
                {
                    Code = 404,
                    data = null,
                    InternalMessage = LanguageAll.Language.Fail,
                    MoreInfo = LanguageAll.Language.Fail,
                    Status = 0,
                    UserMessage = LanguageAll.Language.Fail
                };
        }

        public async Task<InvoiceResult> CheckInvoice(InvoiceInput inv)
        {
            var a = await _invoice.GetInvoice(inv);
            if (a.DataStatus == "00") a.ItemsData.CompanyID = inv.CompanyID;
            return a;
        }

        public async Task<InvoiceAllResult> CheckAllInvoice(InvoiceAllInput inv)
        {
            var a = await _invoice.GetInvoiceAll(inv);
            return a;
        }

        public async Task<InvoiceDataResult> GetInvoiceA(InvoiceInput inv)
        {
            var a = await _invoice.GetInvoiceA(inv);
            return a;
        }

        public async Task<InvoiceDataResult> GetInvoiceAllA(InvoiceAllAInput inv)
        {
            var a = await _invoice.GetInvoiceAllA(inv);
            return a;
        }

        public async Task<CustomerInfoResult> getCustomerInfo(EVNCodeInput inv)
        {
            var a = await _invoice.getCustomerInfo(inv);
            return a;
        }

        public async Task<InvoiceQRCode> getInvoiceByQRCode(InvQrCodeInput inv)
        {
            var a = await _invoice.getInvoiceByQRCode(inv);
            return a;
        }

        public async Task<ResponseOK> GetInvoiceAll(InvoiceAllInput inv)
        {
            var a = await _invoice.GetInvoiceAll(inv);
            if (a.DataStatus == "00")
                return new ResponseOK()
                {
                    Code = 200,
                    data = a,
                    InternalMessage = LanguageAll.Language.Success,
                    MoreInfo = LanguageAll.Language.Success,
                    Status = 1,
                    UserMessage = LanguageAll.Language.Success
                };
            else
                return new ResponseOK()
                {
                    Code = 404,
                    data = a,
                    InternalMessage = LanguageAll.Language.Fail,
                    MoreInfo = LanguageAll.Language.Fail,
                    Status = 0,
                    UserMessage = LanguageAll.Language.Fail
                };
        }

        public async Task<ResponseOK> PayInvoice(PayInput inv)
        {
            var a = await _invoice.PayInvoice(inv);
            if (a.PayStatus == "00")
                return new ResponseOK()
                {
                    Code = 200,
                    data = a,
                    InternalMessage = LanguageAll.Language.Success,
                    MoreInfo = LanguageAll.Language.Success,
                    Status = 1,
                    UserMessage = LanguageAll.Language.Success
                };
            else
                return new ResponseOK()
                {
                    Code = 404,
                    data = a,
                    InternalMessage = LanguageAll.Language.Fail,
                    MoreInfo = LanguageAll.Language.Fail,
                    Status = 0,
                    UserMessage = LanguageAll.Language.Fail
                };
        }

        public async Task<ResponseOK> UndoPayInvoice(InvoiceInput inv)
        {
            var a = await _invoice.UndoPayInvoice(inv);
            if (a.UndoPayStatus == "00")
                return new ResponseOK()
                {
                    Code = 200,
                    data = a,
                    InternalMessage = LanguageAll.Language.Success,
                    MoreInfo = LanguageAll.Language.Success,
                    Status = 1,
                    UserMessage = LanguageAll.Language.Success
                };
            else
                return new ResponseOK()
                {
                    Code = 404,
                    data = a,
                    InternalMessage = LanguageAll.Language.Fail,
                    MoreInfo = LanguageAll.Language.Fail,
                    Status = 0,
                    UserMessage = LanguageAll.Language.Fail
                };
        }
    }
}