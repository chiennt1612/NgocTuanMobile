﻿using Auth.Models;
using Auth.Services.Interfaces;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Paygate.OnePay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils;
using Utils.ExceptionHandling;
using Utils.Models;

namespace Auth.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v1.0/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json", "application/problem+json")]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        #region Properties
        private readonly IDistributedCache _cache;
        private readonly ILogger<InvoiceController> _logger;
        private readonly IStringLocalizer<InvoiceController> _localizer;
        private readonly IAllService _Service;
        private PaygateInfo paygateInfo;
        private IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IInvoiceServices _iInvoiceServices;
        private readonly IInvoiceSaveServices _iInvoiceSaveServices;
        public CompanyConfig companyConfig { get; set; }
        #endregion

        public InvoiceController(IConfiguration _configuration, IEmailSender _emailSender, IDistributedCache _cache,
            ILogger<InvoiceController> _logger, IStringLocalizer<InvoiceController> _localizer,
            IInvoiceServices _iInvoiceServices, IAllService _Service,
            IInvoiceSaveServices _iInvoiceSaveServices)
        {
            this._logger = _logger;
            this._iInvoiceServices = _iInvoiceServices;
            this._iInvoiceSaveServices = _iInvoiceSaveServices;
            this._Service = _Service;
            this._localizer = _localizer;
            this._cache = _cache;
            this._emailSender = _emailSender;
            this._configuration = _configuration;
            paygateInfo = this._configuration.GetSection(nameof(PaygateInfo)).Get<PaygateInfo>();
            this._logger.WriteLog("Starting invoice page");
            companyConfig = this._configuration.GetSection(nameof(CompanyConfig)).Get<CompanyConfig>();
        }

        [HttpPost]
        [Route("[action]/{Page}")]
        public async Task<IActionResult> ListAll(int? Page, [FromBody] InvoiceModel invM)
        {
            InvoiceAllInput inv = new InvoiceAllInput()
            {
                CompanyID = invM.CompanyID,
                CustomerCode = invM.CustomerCode,
                Page = (Page.HasValue ? Page.Value : 1),
                FromDate = (invM.FromDate.HasValue ? invM.FromDate.Value.ToString("MM/dd/yyyy") : ""),
                ToDate = (invM.ToDate.HasValue ? invM.ToDate.Value.ToString("MM/dd/yyyy") : ""),
                PaymentStatus = (invM.PaymentStatus.HasValue ? invM.PaymentStatus.Value.ToString() : "")
            };
            var a = await _iInvoiceServices.GetInvoiceAll(inv);
            _logger.WriteLog($"ListAll {JsonConvert.SerializeObject(inv)}: {a.UserMessage}", "ListAll");
            return Ok(a);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> List([FromBody] InvoiceInput inv)
        {
            var a = await _iInvoiceServices.GetInvoice(inv);
            _logger.WriteLog($"List {JsonConvert.SerializeObject(inv)}: {a.UserMessage}", "List");
            return Ok(a);
        }

        [HttpPost]
        [Route("[action]/{Page}")]
        public async Task<IActionResult> FindInvoice(int? Page, [FromBody] InvoiceFindModel invM)
        {
            List<CompanyInvoice> r = new List<CompanyInvoice>();
            foreach (var companyInfo in companyConfig.Companys)
            {
                var a = new CompanyInvoice()
                {
                    CompanyCode = companyInfo.Info.CompanyCode,
                    CompanyId = companyInfo.Info.CompanyId,
                    CompanyLogo = companyInfo.Info.CompanyLogo,
                    CompanyNameEn = companyInfo.Info.CompanyNameEn,
                    CompanyName = companyInfo.Info.CompanyName,
                    Taxcode = companyInfo.Info.Taxcode,
                    itemsData = new List<ItemsDatum>()
                };
                var contract = User.Claims.Where(c => c.Type == "GetInvoice" && c.Value.StartsWith($"{companyInfo.Info.CompanyId}."));
                int CompanyId = companyInfo.Info.CompanyId;
                string CustomerCode = "";
                foreach (var item in contract)
                {                    
                    var arr = item.Value.Split(".");
                    if (arr.Length > 1)
                    {
                        CustomerCode = CustomerCode + "," + arr[1];
                    }
                    else
                    {
                        CustomerCode = CustomerCode + "," + arr[0];
                    }
                }
                var inv = new InvoiceAllAInput()
                {
                    CompanyID = CompanyId,
                    CustomerCodeList = CustomerCode,
                    FromDate = invM.FromDate.HasValue ? invM.FromDate.Value.ToString("MM/dd/yyyy") : "",
                    ToDate = invM.ToDate.HasValue ? invM.ToDate.Value.ToString("MM/dd/yyyy") : "",
                    Page = Page.HasValue ? Page.Value : 1
                };
                var a2 = await _iInvoiceServices.GetInvoiceAllA(inv);
                if (a2.DataStatus == "00")
                {
                    a.itemsData = a2.ItemsData;
                }
                r.Add(a);
            }

            var a1 = new ResponseOK()
            {
                Code = 200,
                data = r,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success
            };
            return Ok(a1);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> FindInvoiceByQRCode([FromBody] InvQRCodeModel invM)
        {
            string[] a = invM.QRCode.Split("|"); //0900996305|1/004|1|K22TYT1|---|10/06/2022|26145
            int CompanyId = 0;
            for (int i = 0; i < companyConfig.Companys.Count; i++)
            {
                if (companyConfig.Companys[i].Info.Taxcode == a[0])
                {
                    CompanyId = companyConfig.Companys[i].Info.CompanyId;
                    break;
                }
            }

            InvQrCodeInput inv = new InvQrCodeInput()
            {
                CompanyID = CompanyId,
                InvoiceNumber = a[6],
                InvoiceSerial = a[3]
            };
            var a1 = await _iInvoiceServices.getInvoiceByQRCode(inv);
            return Ok(a1);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> List()
        {
            List<ItemsDatum> r = new List<ItemsDatum>();
            foreach (var item in User.Claims.Where(u => u.Type == "GetInvoice"))
            {
                int CompanyId = 0;
                string CustomerCode = "";
                var arr = item.Value.Split(".");
                if (arr.Length > 1)
                {
                    CompanyId = int.Parse(arr[0]);
                    CustomerCode = arr[1];
                }
                else
                {
                    CustomerCode = arr[0];
                }
                var inv = new InvoiceInput()
                {
                    CompanyID = CompanyId,
                    CustomerCode = CustomerCode
                };
                var a = await _iInvoiceServices.GetInvoiceA(inv);
                if (a.DataStatus == "00")
                {
                    r.AddRange(a.ItemsData);
                }
            }
            var a1 = new ResponseOK()
            {
                Code = 200,
                data = r,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success
            };
            return Ok(a1);
        }

        [HttpPost]
        [Route("[action]/{Page}")]
        public async Task<IActionResult> InvoiceSave(int? Page, [FromBody] SearchDateModel inv)
        {
            var r = await _iInvoiceSaveServices.InvoceSaveGetListAsync(Page, 10, new SearchDateModel() { FromDate = inv.FromDate, ToDate = inv.ToDate});
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = r
            }); 
        }
        
        [HttpGet]
        [Route("[action]/{Id}")]
        public async Task<IActionResult> InvoiceDelete(long Id)
        {
            var r = await _iInvoiceSaveServices.DeleteAsync(Id);
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = r
            });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Pay([FromBody] PayInputModel inv)
        {
            dynamic rd;
            if (inv.IsAgree)
            {
                InvoiceInput invInput = new InvoiceInput()
                {
                    CompanyID = inv.CompanyID,
                    CustomerCode = inv.CustomerCode
                };
                var a = await _iInvoiceServices.CheckInvoice(invInput);
                if (a != null)
                {
                    var invq = a.ItemsData.InvList.Where(u => u.InvCode == inv.InvCode).FirstOrDefault();
                    if (invq != null)
                    {
                        if (invq.InvAmount == inv.InvAmount)
                        {
                            var _contact = new Contact()
                            {
                                Address = inv.CustomerCode,
                                CompanyName = inv.CustomerCode,
                                ContactDate = DateTime.Now,
                                Description = $"2_{inv.CustomerCode}_{inv.InvCode}_{inv.InvAmount}",
                                Email = inv.CustomerCode,
                                Fullname = inv.CustomerCode,
                                IsCompany = false,
                                Mobile = inv.InvCode.ToString(),
                                Price = inv.InvAmount,
                                ServiceId = null,
                                StatusId = 0,
                                UserId = -1,
                                PaymentMethod = 3,
                                IsAgree = true,
                                IsSave = inv.IsSave
                            };
                            if (User.Identity.IsAuthenticated)
                            {
                                _contact.Fullname = User.Claims.GetClaimValue("Fullname");
                                _contact.Email = User.Claims.GetClaimValue(ClaimTypes.Email);
                                _contact.Mobile = User.Claims.GetClaimValue(ClaimTypes.Name);
                                _contact.Address = User.Claims.GetClaimValue("Address");
                                _contact.UserId = long.Parse(User.Claims.GetClaimValue(ClaimTypes.NameIdentifier));
                            }
                            var r = await _Service.contactServices.AddAsync(_contact);

                            if (r != null)
                            {
                                if (r.IsSave.HasValue && r.IsSave.Value)
                                {
                                    var orderSave = new InvoiceSave()
                                    {
                                        Id = r.Id,
                                        Address = r.Address,
                                        CustomerCode = inv.CustomerCode,
                                        CustomerName = r.Fullname,
                                        InvAmount = invq.InvAmount,
                                        InvAmountWithoutTax = invq.InvAmountWithoutTax,
                                        InvCode = invq.InvCode.ToString(),
                                        InvDate = invq.InvDate,
                                        InvNumber = invq.InvNumber,
                                        InvRemarks = invq.InvRemarks,
                                        InvSerial = invq.InvSerial,
                                        MaSoBiMat = invq.MaSoBiMat,
                                        PaymentStatus = 0,
                                        TaxPer = invq.TaxPer
                                    };
                                    await _iInvoiceSaveServices.AddAsync(orderSave);
                                }
                                _logger.LogInformation($"Send payment-invoice is success: {_contact.Fullname}");
                                PaymentIn t = new PaymentIn()
                                {
                                    vpc_Amount = _contact.Price.ToString(),
                                    vpc_Customer_Email = "",
                                    vpc_Customer_Id = User.Claims.GetClaimValue(ClaimTypes.NameIdentifier),
                                    vpc_Customer_Phone = "",
                                    vpc_MerchTxnRef = $"{(r.Id + Paygate.OnePay.Tools.StartIdOrder).ToString()}",//.{inv.InvoiceNo}
                                    vpc_OrderInfo = $"2_{inv.CustomerCode}_{inv.InvCode}_{inv.InvAmount}",
                                    vpc_SHIP_City = "Han",
                                    vpc_SHIP_Country = "VN",
                                    vpc_SHIP_Provice = "Han",
                                    vpc_SHIP_Street01 = ""
                                };
                                VPCRequest conn = new VPCRequest(paygateInfo, _logger);
                                var url = conn.CreatePay(HttpContext, paygateInfo, t);
                                _logger.LogInformation(url);
                                rd = new PayModel() { Url = url, order = r };

                                return Ok(new ResponseOK()
                                {
                                    Code = 200,
                                    InternalMessage = LanguageAll.Language.Success,
                                    MoreInfo = LanguageAll.Language.Success,
                                    Status = 1,
                                    UserMessage = LanguageAll.Language.Success,
                                    data = rd
                                });
                            }
                        }
                    }
                }
            }
            return StatusCode(StatusCodes.Status200OK, new ResponseOK()
            {
                Code = 400,
                InternalMessage = LanguageAll.Language.Fail,
                MoreInfo = LanguageAll.Language.Fail,
                Status = 0,
                UserMessage = LanguageAll.Language.Fail,
                data = null
            });
        }

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<IActionResult> CheckPay(CheckPayInput inv)
        //{
        //    var a = await _iInvoiceServices.CheckPayInvoice(inv);
        //    _logger.WriteLog($"CheckPay {JsonConvert.SerializeObject(inv)}: {JsonConvert.SerializeObject(a)}", "CheckPay");
        //    return Ok(a);
        //}

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<IActionResult> UndoPay(InvoiceInput inv)
        //{
        //    var a = await _iInvoiceServices.UndoPayInvoice(inv);
        //    _logger.WriteLog($"UndoPay {JsonConvert.SerializeObject(inv)}: {JsonConvert.SerializeObject(a)}", "UndoPay");
        //    return Ok(a);
        //}
    }
}
