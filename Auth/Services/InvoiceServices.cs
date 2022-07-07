﻿using Auth.Services.Interfaces;
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

        public async Task<ResponseOK> GetInvoice(InvoiceInput inv)
        {
            var a = await _invoice.GetInvoice(inv);
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

        public async Task<InvoiceResult> CheckInvoice(InvoiceInput inv)
        {
            var a = await _invoice.GetInvoice(inv);
            return a;
        }

        public async Task<InvoiceDataResult> GetInvoiceA(InvoiceInput inv)
        {
            var a = await _invoice.GetInvoiceA(inv);
            return a;
        }

        public async Task<CustomerInfoResult> getCustomerInfo(EVNCodeInput inv)
        {
            var a = await _invoice.getCustomerInfo(inv);
            return a;
        }

        public async Task<ResponseOK> getInvoiceByQRCode(InvQrCodeInput inv)
        {
            var a = await _invoice.getInvoiceByQRCode(inv);
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

        public async Task<ResponseOK> GetInvoiceAll(InvoiceAllInput inv)
        {
            var a = await _invoice.GetInvoiceAll(inv);
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

        public async Task<ResponseOK> PayInvoice(PayInput inv)
        {
            var a = await _invoice.PayInvoice(inv);
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

        public async Task<ResponseOK> UndoPayInvoice(InvoiceInput inv)
        {
            var a = await _invoice.UndoPayInvoice(inv);
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
    }
}
