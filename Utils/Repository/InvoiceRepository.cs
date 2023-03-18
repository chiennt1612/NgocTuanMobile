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

        #region Invoice
        public async Task<InvoiceResult> GetInvoice(InvoiceInput inv)           //Index: 0
        {
            InvoiceResult a = default;
            try
            {
                a = await Tools.APIRequest<InvoiceResult, InvoiceInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[0], inv);
                if (ilogger != null) ilogger.LogInformation($"GetInvoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"GetInvoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }
        public async Task<InvoiceQRCode> getInvoiceByQRCode(InvQrCodeInput inv) //Index: 9
        {
            InvoiceQRCode a = default;
            try
            {
                a = await Tools.APIRequest<InvoiceQRCode, InvQrCodeInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[9], inv);
                if (ilogger != null) ilogger.LogInformation($"GetInvoiceHistory {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"GetInvoiceHistory {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }
        public async Task<PayResult> PayInvoice(PayInput inv)                   //Index: 1
        {
            PayResult a = default;
            try
            {
                a = await Tools.APIRequest<PayResult, PayInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[1], inv);
                if (ilogger != null) ilogger.LogInformation($"PayInvoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"PayInvoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }
        public async Task<PayResult> CheckPayInvoice(CheckPayInput inv)         //Index: 2
        {
            PayResult a = default;
            try
            {
                a = await Tools.APIRequest<PayResult, CheckPayInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[2], inv);
                if (ilogger != null) ilogger.LogInformation($"Check pay status invoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"Check pay status invoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }
        public async Task<UndoPayResult> UndoPayInvoice(InvoiceInput inv)       //Index: 3
        {
            UndoPayResult a = default;
            try
            {
                a = await Tools.APIRequest<UndoPayResult, InvoiceInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[3], inv);
                if (ilogger != null) ilogger.LogInformation($"UndoPayInvoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"UndoPayInvoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }
        #endregion

        #region Customer
        public async Task<ContractResult> GetContract(ContractInput inv)        //Index: 5
        {
            ContractResult a = default;
            try
            {
                a = await Tools.APIRequest<ContractResult, ContractInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[5], inv);
                if (ilogger != null) ilogger.LogInformation($"getContractAllList {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"getContractAllList {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }
        public async Task<InvoiceAllResult> GetInvoiceAll(InvoiceAllInput inv)  //Index: 4
        {
            InvoiceAllResult a = default;
            try
            {
                a = await Tools.APIRequest<InvoiceAllResult, InvoiceAllInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[4], inv);
                if (ilogger != null) ilogger.LogInformation($"GetInvoiceHistory {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"GetInvoiceHistory {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }
        public async Task<InvoiceDataResult> GetInvoiceA(InvoiceInput inv)      //Index: 7
        {
            InvoiceDataResult a = default;
            try
            {
                a = await Tools.APIRequest<InvoiceDataResult, InvoiceInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[7], inv);
                //if (ilogger != null) ilogger.LogInformation($"GetInvoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"GetInvoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }
        public async Task<InvoiceDataResult> GetInvoiceAllA(InvoiceAllAInput inv)//Index: 6
        {
            InvoiceDataResult a = default;
            try
            {
                a = await Tools.APIRequest<InvoiceDataResult, InvoiceAllAInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[6], inv);
                //if (ilogger != null) ilogger.LogInformation($"GetInvoice {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"GetInvoice {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }
        public async Task<CustomerInfoResult> getCustomerInfo(EVNCodeInput inv) //Index: 8
        {
            CustomerInfoResult a = default;
            try
            {
                a = await Tools.APIRequest<CustomerInfoResult, EVNCodeInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[8], inv);
                if (ilogger != null) ilogger.LogInformation($"GetInvoiceHistory {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"GetInvoiceHistory {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }
        #endregion

        #region Staff
        public async Task<StaffInfoResult> getStaffInfo(StaffCodeInput inv)     //Index: 4
        {
            StaffInfoResult a = default;
            try
            {
                a = await Tools.APIRequest<StaffInfoResult, StaffCodeInput>(ilogger,
                    companyConfig.Companys[inv.CompanyID].Config.APIUrl,
                    companyConfig.Companys[inv.CompanyID].Config.APIToken,
                    companyConfig.Companys[inv.CompanyID].Config.APIFunctions[4], inv);
                if (ilogger != null) ilogger.LogInformation($"GetInvoiceHistory {JsonConvert.SerializeObject(inv)} is result {JsonConvert.SerializeObject(a)}");
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"GetInvoiceHistory {JsonConvert.SerializeObject(inv)} is error {ex.Message}");
            }
            return a;
        }
        #endregion
    }
}
