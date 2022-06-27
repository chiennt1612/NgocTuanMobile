using Auth.Models;
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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils;
using Utils.ExceptionHandling;
using Utils.Models;
using Utils.Repository.Interfaces;

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
        #endregion

        public InvoiceController(IConfiguration _configuration, IEmailSender _emailSender, IDistributedCache _cache,
            ILogger<InvoiceController> _logger, IStringLocalizer<InvoiceController> _localizer, IInvoiceServices _iInvoiceServices, IAllService _Service)
        {
            this._logger = _logger;
            this._iInvoiceServices = _iInvoiceServices;
            this._Service = _Service;
            this._localizer = _localizer;
            this._cache = _cache;
            this._emailSender = _emailSender;
            this._configuration = _configuration;
            paygateInfo = this._configuration.GetSection(nameof(PaygateInfo)).Get<PaygateInfo>();
            this._logger.WriteLog("Starting invoice page");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListAll(InvoiceAllInput inv)
        {
            var a = await _iInvoiceServices.GetInvoiceAll(inv);
            _logger.WriteLog($"ListAll {JsonConvert.SerializeObject(inv)}: {a.UserMessage}", "ListAll");
            return Ok(a);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> List(InvoiceInput inv)
        {
            var a = await _iInvoiceServices.GetInvoice(inv);
            _logger.WriteLog($"List {JsonConvert.SerializeObject(inv)}: {a.UserMessage}", "List");
            return Ok(a);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Pay(PayInputModel inv)
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
                    var invq = a.ItemsData.InvList.Where(u => u.InvCode == inv.InvoiceNo).FirstOrDefault();
                    if (invq != null)
                    {
                        if (invq.InvAmount == inv.InvoiceAmount)
                        {
                            var _contact = new Contact()
                            {
                                Address = inv.CustomerCode,
                                CompanyName = inv.CustomerCode,
                                ContactDate = DateTime.Now,
                                Description = $"2_{inv.CustomerCode}_{inv.InvoiceNo}_{inv.InvoiceAmount}",
                                Email = inv.CustomerCode,
                                Fullname = inv.CustomerCode,
                                IsCompany = false,
                                Mobile = inv.InvoiceNo,
                                Price = inv.InvoiceAmount,
                                ServiceId = null,
                                StatusId = 0,
                                UserId = -1,
                                PaymentMethod = 3,
                                IsAgree = true
                            };

                            var r = await _Service.contactServices.AddAsync(_contact);

                            if (r != null)
                            {
                                _logger.LogInformation($"Send payment-invoice is success: {_contact.Fullname}");
                                PaymentIn t = new PaymentIn()
                                {
                                    vpc_Amount = _contact.Price.ToString(),
                                    vpc_Customer_Email = "",
                                    vpc_Customer_Id = User.Claims.GetClaimValue(ClaimTypes.NameIdentifier),
                                    vpc_Customer_Phone = "",
                                    vpc_MerchTxnRef = $"{(r.Id + Paygate.OnePay.Tools.StartIdOrder).ToString()}",//.{inv.InvoiceNo}
                                    vpc_OrderInfo = $"2_{inv.CustomerCode}_{inv.InvoiceNo}_{inv.InvoiceAmount}",
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
