using Auth.Models;
using Auth.Services.Interfaces;
using EntityFramework.API.Entities;
using EntityFramework.API.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly IDistributedCache _cache;
        private readonly ILogger<InvoiceController> _logger;
        private readonly IStringLocalizer<InvoiceController> _localizer;
        private readonly IAllService _Service;
        private PaygateInfo paygateInfo;
        private IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        public CompanyConfig companyConfig { get; set; }
        #endregion

        public InvoiceController(IConfiguration _configuration, IEmailSender _emailSender, IDistributedCache _cache,
            ILogger<InvoiceController> _logger, IStringLocalizer<InvoiceController> _localizer, IAllService _Service,
            UserManager<AppUser> userManager)
        {
            this._logger = _logger;
            this._Service = _Service;
            this._localizer = _localizer;
            this._cache = _cache;
            this._emailSender = _emailSender;
            this._configuration = _configuration;
            paygateInfo = this._configuration.GetSection(nameof(PaygateInfo)).Get<PaygateInfo>();
            this._logger.WriteLog("Starting invoice page");
            companyConfig = this._configuration.GetSection(nameof(CompanyConfig)).Get<CompanyConfig>();
            _userManager = userManager;
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
            string _key = $"{inv.CompanyID}.{inv.CustomerCode}.{inv.Page}.{inv.FromDate}.{inv.ToDate}.{inv.PaymentStatus}".ToMD5Hash();
            ResponseOK a = await _cache.GetAsync<ResponseOK>(_key);
            if (a == null)
            {
                a = await _Service.invoiceServices.GetInvoiceAll(inv);
                await _cache.SetAsync(_key, a);
            }
            _logger.WriteLog($"ListAll {JsonConvert.SerializeObject(inv)}: {a.UserMessage}", "ListAll");
            return Ok(a);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> List([FromBody] InvoiceInput inv)
        {
            var a = await _Service.invoiceServices.GetInvoice(inv);
            _logger.WriteLog($"List {JsonConvert.SerializeObject(inv)}: {a.UserMessage}", "List");
            return Ok(a);
        }

        [HttpPost]
        [Route("[action]/{Page}")]
        public async Task<IActionResult> FindInvoice(int? Page, [FromBody] InvoiceFindModel invM)
        {
            List<CompanyInvoice> r = new List<CompanyInvoice>();
            int itemsCount = 0;
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
                var user1 = await _userManager.GetUserAsync(HttpContext.User);
                var claims = await _userManager.GetClaimsAsync(user1);
                var contract = claims.Where(c => c.Type == "GetInvoice" && c.Value.StartsWith($"{companyInfo.Info.CompanyId}."));
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
                    Page = Page.HasValue ? Page.Value : 1,
                    PaymentStatus = invM.PaymentStatus.HasValue ? invM.PaymentStatus.Value.ToString() : "2"
                };
                var a2 = await _Service.invoiceServices.GetInvoiceAllA(inv);
                if (a2.DataStatus == "00")
                {
                    for (var j = 0; j < a2.ItemsData.Count; j++)
                    {
                        a2.ItemsData[j].CompanyId = CompanyId;
                    }
                    a.itemsData = a2.ItemsData;
                    itemsCount = itemsCount + int.Parse(a2.Message);
                }
                r.Add(a);
            }

            var a1 = new ResponseOK()
            {
                Code = 200,
                data = new
                {
                    itemsCount = itemsCount,
                    itemsList = r
                },
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
            int i = 0;
            bool found = false;
            while (i < companyConfig.Companys.Count && !found)
            {
                if (companyConfig.Companys[i].Info.Taxcode == a[0]) found = true;
                i++;
            }
            if (found)
                i = i - 1;
            else
                return Ok(
                    new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.NotFound,
                        MoreInfo = LanguageAll.Language.NotFound,
                        Status = 0,
                        UserMessage = LanguageAll.Language.NotFound,
                        data = null
                    });

            InvQrCodeInput inv = new InvQrCodeInput()
            {
                CompanyID = companyConfig.Companys[i].Info.CompanyId,
                InvoiceNumber = a[6],
                InvoiceSerial = a[3]
            };
            var a1 = await _Service.invoiceServices.getInvoiceByQRCode(inv);
            if (a1.DataStatus == "00")
            {
                a1.CompanyInfo = companyConfig.Companys[i].Info;
                return Ok(new ResponseOK()
                {
                    Code = 200,
                    data = a1,
                    InternalMessage = LanguageAll.Language.Success,
                    MoreInfo = LanguageAll.Language.Success,
                    Status = 1,
                    UserMessage = LanguageAll.Language.Success
                });
            }
            else
                return Ok(new ResponseOK()
                {
                    Code = 404,
                    data = null,
                    InternalMessage = LanguageAll.Language.Fail,
                    MoreInfo = LanguageAll.Language.Fail,
                    Status = 0,
                    UserMessage = LanguageAll.Language.Fail
                });
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> List()
        {
            List<ItemsDatum> r = new List<ItemsDatum>();
            var user1 = await _userManager.GetUserAsync(HttpContext.User);
            var claims = await _userManager.GetClaimsAsync(user1);
            foreach (var item in claims.Where(u => u.Type == "GetInvoice"))
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
                var a = await _Service.invoiceServices.GetInvoiceA(inv);
                if (a.DataStatus == "00")
                {
                    for (var j = 0; j < a.ItemsData.Count; j++)
                    {
                        a.ItemsData[j].CompanyId = CompanyId;
                    }
                    r.AddRange(a.ItemsData);
                }
            }
            ResponseOK a1;
            if (r.Count > 0)
                a1 = new ResponseOK()
                {
                    Code = 200,
                    data = r,
                    InternalMessage = LanguageAll.Language.Success,
                    MoreInfo = LanguageAll.Language.Success,
                    Status = 1,
                    UserMessage = LanguageAll.Language.Success
                };
            else
                a1 = new ResponseOK()
                {
                    Code = 404,
                    data = null,
                    InternalMessage = LanguageAll.Language.NotFound,
                    MoreInfo = LanguageAll.Language.NotFound,
                    Status = 0,
                    UserMessage = LanguageAll.Language.NotFound
                };
            return Ok(a1);
        }

        [HttpPost]
        [Route("[action]/{Page}")]
        public async Task<IActionResult> InvoiceSave(int? Page, [FromBody] InvoiceSaveSearchModel inv)
        {
            var userId = long.Parse(HttpContext.User.Claims.Where(u => u.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value);
            var r = await _Service.iInvoiceSaveServices.InvoceSaveGetListAsync(Page, 10, new InvoiceSaveSearchModelA() { FromDate = inv.FromDate, ToDate = inv.ToDate, CustomerCode = inv.CustomerCode, UserId = userId });
            var c = await _Service.iInvoiceSaveServices.GetCountAsync(new InvoiceSaveSearchModelA() { FromDate = inv.FromDate, ToDate = inv.ToDate, CustomerCode = inv.CustomerCode, UserId = userId });
            if (c < 1)
            {
                return Ok(new ResponseOK()
                {
                    Code = 404,
                    InternalMessage = LanguageAll.Language.NotFound,
                    MoreInfo = LanguageAll.Language.NotFound,
                    Status = 0,
                    UserMessage = LanguageAll.Language.NotFound,
                    data = null
                });
            }
            else
                return Ok(new ResponseOK()
                {
                    Code = 200,
                    InternalMessage = LanguageAll.Language.Success,
                    MoreInfo = LanguageAll.Language.Success,
                    Status = 1,
                    UserMessage = LanguageAll.Language.Success,
                    data = new
                    {
                        itemsCount = c,
                        itemsList = r/*,
                        companyInfo = companyConfig.Companys[0].Info*/
                    }
                });
        }

        [HttpGet]
        [Route("[action]/{Id}")]
        public async Task<IActionResult> InvoiceDelete(long Id)
        {
            var r = await _Service.iInvoiceSaveServices.DeleteAsync(Id);
            if (r)
                return Ok(new ResponseOK()
                {
                    Code = 200,
                    InternalMessage = LanguageAll.Language.Success,
                    MoreInfo = LanguageAll.Language.Success,
                    Status = 1,
                    UserMessage = LanguageAll.Language.Success,
                    data = r
                });
            else
                return Ok(new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.Fail,
                    MoreInfo = LanguageAll.Language.Fail,
                    Status = 0,
                    UserMessage = LanguageAll.Language.Fail,
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
                var a = await _Service.invoiceServices.CheckInvoice(invInput);
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
                                        TaxPer = invq.TaxPer,
                                        UserId = long.Parse(User.Claims.GetClaimValue(ClaimTypes.NameIdentifier)),
                                        CompanyCode = companyConfig.Companys[inv.CompanyID].Info.CompanyCode,
                                        CompanyId = companyConfig.Companys[inv.CompanyID].Info.CompanyId,
                                        CompanyLogo = companyConfig.Companys[inv.CompanyID].Info.CompanyLogo,
                                        CompanyName = companyConfig.Companys[inv.CompanyID].Info.CompanyName,
                                        CompanyNameEn = companyConfig.Companys[inv.CompanyID].Info.CompanyNameEn,
                                        Taxcode = companyConfig.Companys[inv.CompanyID].Info.Taxcode
                                    };
                                    await _Service.iInvoiceSaveServices.AddAsync(orderSave);
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
