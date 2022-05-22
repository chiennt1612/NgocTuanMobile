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

        public async Task<PayResult> CheckPayInvoice(CheckPayInput inv)
        {
            PayResult a = default;
            try
            {
                a = await Tools.APIRequest<PayResult, CheckPayInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[2], inv);
                ilogger.LogInformation($"Check pay status invoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                ilogger.LogError($"Check pay status invoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }

        public async Task<InvoiceResult> GetInvoice(InvoiceInput inv)
        {
            InvoiceResult a = default;
            try
            {
                a = await Tools.APIRequest<InvoiceResult, InvoiceInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[0], inv);
                ilogger.LogInformation($"GetInvoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                ilogger.LogError($"GetInvoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }

        public async Task<InvoiceAllResult> GetInvoiceAll(InvoiceAllInput inv)
        {
            InvoiceAllResult a = default;
            try
            {
                a = await Tools.APIRequest<InvoiceAllResult, InvoiceAllInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[4], inv);
                ilogger.LogInformation($"GetInvoiceHistory {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                ilogger.LogError($"GetInvoiceHistory {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }

        public async Task<PayResult> PayInvoice(PayInput inv)
        {
            PayResult a = default;
            try
            {
                a = await Tools.APIRequest<PayResult, PayInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[1], inv);
                ilogger.LogInformation($"PayInvoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                ilogger.LogError($"PayInvoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }

        public async Task<UndoPayResult> UndoPayInvoice(InvoiceInput inv)
        {
            UndoPayResult a = default;
            try
            {
                a = await Tools.APIRequest<UndoPayResult, InvoiceInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[2], inv);
                ilogger.LogInformation($"UndoPayInvoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                ilogger.LogError($"UndoPayInvoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }

        public async Task<ContractResult> GetContract(ContractInput inv)
        {
            ContractResult a = default;
            try
            {
                a = await Tools.APIRequest<ContractResult, ContractInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[5], inv);
                ilogger.LogInformation($"getContractAllList {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                ilogger.LogError($"getContractAllList {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }
    }
}
