using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Utils.Models;
using Utils.Repository.Interfaces;

namespace Utils.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private ILogger<InvoiceRepository> ilogger;
        private IConfiguration configuration;
        public CompanyConfig companyConfig { get; set; }
        //Task<T> APIRequest<T, T1>(ILogger _logger, string APIUrl, string APIToken, string functionName, T1 pzData)
        public InvoiceRepository(ILogger<InvoiceRepository> ilogger, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.ilogger = ilogger;
            companyConfig = this.configuration.GetSection(nameof(CompanyConfig)).Get<CompanyConfig>();
        }

        public async Task<PayResult> CheckPayInvoice(CheckPayInput inv, int Company = 0)
        {
            PayResult a = default;
            try
            {
                a = await Tools.APIRequest<PayResult, CheckPayInput>(ilogger,
                    companyConfig.Companys[Company].Config.APIUrl,
                    companyConfig.Companys[Company].Config.APIToken,
                    companyConfig.Companys[Company].Config.APIFunctions[2], inv);
                ilogger.LogInformation($"Check pay status invoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                ilogger.LogError($"Check pay status invoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }

        public async Task<InvoiceResult> GetInvoice(InvoiceInput inv, int Company = 0)
        {
            InvoiceResult a = default;
            try
            {
                a = await Tools.APIRequest<InvoiceResult, InvoiceInput>(ilogger,
                    companyConfig.Companys[Company].Config.APIUrl,
                    companyConfig.Companys[Company].Config.APIToken,
                    companyConfig.Companys[Company].Config.APIFunctions[0], inv);
                ilogger.LogInformation($"GetInvoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                ilogger.LogError($"GetInvoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }

        public async Task<InvoiceAllResult> GetInvoiceAll(InvoiceAllInput inv, int Company = 0)
        {
            InvoiceAllResult a = default;
            try
            {
                a = await Tools.APIRequest<InvoiceAllResult, InvoiceAllInput>(ilogger,
                    companyConfig.Companys[Company].Config.APIUrl,
                    companyConfig.Companys[Company].Config.APIToken,
                    companyConfig.Companys[Company].Config.APIFunctions[4], inv);
                ilogger.LogInformation($"GetInvoiceHistory {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                ilogger.LogError($"GetInvoiceHistory {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }

        public async Task<PayResult> PayInvoice(PayInput inv, int Company = 0)
        {
            PayResult a = default;
            try
            {
                a = await Tools.APIRequest<PayResult, PayInput>(ilogger,
                    companyConfig.Companys[Company].Config.APIUrl,
                    companyConfig.Companys[Company].Config.APIToken,
                    companyConfig.Companys[Company].Config.APIFunctions[1], inv);
                ilogger.LogInformation($"PayInvoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                ilogger.LogError($"PayInvoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }

        public async Task<UndoPayResult> UndoPayInvoice(InvoiceInput inv, int Company = 0)
        {
            UndoPayResult a = default;
            try
            {
                a = await Tools.APIRequest<UndoPayResult, InvoiceInput>(ilogger,
                    companyConfig.Companys[Company].Config.APIUrl,
                    companyConfig.Companys[Company].Config.APIToken,
                    companyConfig.Companys[Company].Config.APIFunctions[2], inv);
                ilogger.LogInformation($"UndoPayInvoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                ilogger.LogError($"UndoPayInvoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }

    }
}
