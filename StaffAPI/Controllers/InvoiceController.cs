﻿using EntityFramework.API.Entities;
using EntityFramework.API.Entities.Identity;
using EntityFramework.API.Entities.Ordering;
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
using StaffAPI.Models;
using StaffAPI.Models.Invoice;
using StaffAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils;
using Utils.ExceptionHandling;
using Utils.Models;

namespace StaffAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("[controller]")]
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
        public ICompanyConfig companyConfig;
        private List<string> invoiceCacheKeys;
        #endregion

        public InvoiceController(IConfiguration _configuration, IEmailSender _emailSender, IDistributedCache _cache,
            ILogger<InvoiceController> _logger, IStringLocalizer<InvoiceController> _localizer, IAllService _Service,
            UserManager<AppUser> userManager, ICompanyConfig companyConfig)
        {
            this._logger = _logger;
            this._Service = _Service;
            this._localizer = _localizer;
            this._cache = _cache;
            this._emailSender = _emailSender;
            this._configuration = _configuration;
            paygateInfo = this._configuration.GetSection(nameof(PaygateInfo)).Get<PaygateInfo>();
            this._logger.WriteLog(_configuration, "Starting invoice page");
            this.companyConfig = companyConfig;
            _userManager = userManager;

            var b = _cache.GetAsync<List<string>>("InvoiceCacheKeys").GetAwaiter();
            invoiceCacheKeys = b.GetResult();
            if (invoiceCacheKeys == null)
            {
                invoiceCacheKeys = new List<string>();
            }
        }

        #region Invoice
        // Listing for Invoice of the contract in area

        // Finding the invoice by customer code/ contract code
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> FindInvoiceByQRCode([FromBody] InvQRCodeModel invM)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.WriteLog(_configuration, $"FindInvoiceByQRCode {JsonConvert.SerializeObject(invM)}", "FindInvoiceByQRCode");
            string _key = $"FindInvoiceByQRCode{invM.QRCode}".ToMD5Hash();
            //if (!invoiceCacheKeys.Contains(_key))
            //{
            //    invoiceCacheKeys.Add(_key);
            //    await _cache.InvoiceSetAsync("InvoiceCacheKeys", invoiceCacheKeys);
            //}
            ResponseOK a2 = await _cache.GetAsync<ResponseOK>(_key);
            if (a2 == null)
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
                {
                    a2 = new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.NotFound,
                        MoreInfo = LanguageAll.Language.NotFound,
                        Status = 0,
                        UserMessage = LanguageAll.Language.NotFound,
                        data = null
                    };
                    await _cache.InvoiceSetAsync(_key, a2);
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return Ok(a2);
                }


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
                    a1.ItemsData.Link = $"https://nuocngoctuan.com/Account/InvoiceView?reservationCode={a1.ItemsData.MaSoBiMat}&supplierTaxCode={companyConfig.Companys[inv.CompanyID].Info.Taxcode}& invoiceNo={a1.ItemsData.InvSerial}{a1.ItemsData.InvNumber}& invoiceType={_configuration["CompanyConfig:InvoiceType"]}";
                    a2 = new ResponseOK()
                    {
                        Code = 200,
                        data = a1,
                        InternalMessage = LanguageAll.Language.Success,
                        MoreInfo = LanguageAll.Language.Success,
                        Status = 1,
                        UserMessage = LanguageAll.Language.Success
                    };
                }
                else
                    a2 = new ResponseOK()
                    {
                        Code = 404,
                        data = null,
                        InternalMessage = LanguageAll.Language.Fail,
                        MoreInfo = LanguageAll.Language.Fail,
                        Status = 0,
                        UserMessage = LanguageAll.Language.Fail
                    };
                await _cache.InvoiceSetAsync(_key, a2);
                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                return Ok(a2);
            }

            a2 = new ResponseOK()
            {
                Code = 400,
                InternalMessage = LanguageAll.Language.NotFound,
                MoreInfo = LanguageAll.Language.NotFound,
                Status = 0,
                UserMessage = LanguageAll.Language.NotFound,
                data = null
            };
            await _cache.InvoiceSetAsync(_key, a2);
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return Ok(a2);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> List([FromBody] InvoiceInput inv)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            string _key = $"List{inv.CompanyID}.{inv.CustomerCode}".ToMD5Hash();
            if (!invoiceCacheKeys.Contains(_key))
            {
                invoiceCacheKeys.Add(_key);
                await _cache.InvoiceSetAsync("InvoiceCacheKeys", invoiceCacheKeys);
            }
            ResponseOK /*a = await _cache.GetAsync<ResponseOK>(_key);
            if (a == null)
            {*/
                a = await _Service.invoiceServices.GetInvoice(inv);
            /* await _cache.InvoiceSetAsync(_key, a);
         }*/
            _logger.WriteLog(_configuration, $"List {JsonConvert.SerializeObject(inv)}: {a.UserMessage}", "List");
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return Ok(a);
        }
       
        // Payment invoice by cash
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Pay([FromBody] PayInputModel inv)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Pay {JsonConvert.SerializeObject(inv)}");
            //dynamic rd;
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
                            var user1 = await _userManager.GetUserAsync(HttpContext.User);
                            string StaffCode = user1.UserName;
                            string OnePayID = DateTime.UtcNow.Ticks.ToString();
                            int PaymentStatus = 0;
                            int StatusId = 0;

                            // Gạch nợ
                            var a1 = await _Service.invoiceServices.PayInvoiceByStaff(
                                new PayInputByStaff()
                                {
                                    CompanyID = inv.CompanyID,
                                    CustomerCode = inv.CustomerCode,
                                    InvoiceAmount = int.Parse(inv.InvAmount.ToString()),
                                    InvoiceNo = inv.InvCode.ToString(),
                                    OnePayID = OnePayID,
                                    StaffCode = user1.UserName
                                });
                            if (a1.Code.Value == 200)
                            {
                                StatusId = 4;
                                PaymentStatus = 1;
                            }

                            var _contact = new Contact()
                            {
                                Address = inv.CustomerCode,
                                CompanyName = inv.CustomerCode,
                                ContactDate = DateTime.Now,
                                Description = "{\"StaffCode\":\"" + StaffCode + "\", \"OnePayID\":\"" + OnePayID + "\", \"CustomerCode\":\""+ inv.CustomerCode + "\",\"InvCode\":\"" + inv.InvCode.ToString() + "\",\"InvAmount\":\"" + inv.InvAmount.ToString() + "\"}",
                                Email = inv.CustomerCode,
                                Fullname = inv.CustomerCode,
                                IsCompany = false,
                                Mobile = inv.InvCode.ToString(),
                                Price = inv.InvAmount,
                                ServiceId = null,
                                StatusId = StatusId,
                                UserId = user1.Id,
                                PaymentMethod = 3,
                                IsAgree = true,
                                IsSave = inv.IsSave
                            };
                            _contact.Fullname = User.Claims.GetClaimValue("Fullname");
                            _contact.Email = User.Claims.GetClaimValue(ClaimTypes.Email);
                            _contact.Mobile = User.Claims.GetClaimValue(ClaimTypes.Name);
                            _contact.Address = User.Claims.GetClaimValue("Address");

                            var r = await _Service.contactServices.AddAsync(_contact);
                            await _cache.InvoiceRemoveAsync(invoiceCacheKeys);
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
                                        InvRemarks = "{\"StaffCode\":\"" + StaffCode + "\", \"OnePayID\":\"" + OnePayID + "\"}",
                                        InvSerial = invq.InvSerial,
                                        MaSoBiMat = invq.MaSoBiMat,
                                        PaymentStatus = PaymentStatus,
                                        TaxPer = invq.TaxPer,
                                        UserId = user1.Id,
                                        CompanyCode = companyConfig.Companys[inv.CompanyID].Info.CompanyCode,
                                        CompanyId = companyConfig.Companys[inv.CompanyID].Info.CompanyId,
                                        CompanyLogo = companyConfig.Companys[inv.CompanyID].Info.CompanyLogo,
                                        CompanyName = companyConfig.Companys[inv.CompanyID].Info.CompanyName,
                                        CompanyNameEn = companyConfig.Companys[inv.CompanyID].Info.CompanyNameEn,
                                        Taxcode = companyConfig.Companys[inv.CompanyID].Info.Taxcode,
                                        Link = $"https://nuocngoctuan.com/Account/InvoiceView?reservationCode={invq.MaSoBiMat}&supplierTaxCode={companyConfig.Companys[inv.CompanyID].Info.Taxcode}& invoiceNo={invq.InvSerial}{invq.InvNumber}& invoiceType={_configuration["CompanyConfig:InvoiceType"]}"
                                    };
                                    await _Service.iInvoiceSaveServices.AddAsync(orderSave);
                                }

                                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                                return Ok(a1);
                            }
                        }
                    }
                }
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> PayCheck([FromBody] PayCheckModel inv)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Pay {JsonConvert.SerializeObject(inv)}");
            //dynamic rd;
            if (inv.IsAgree)
            {
                var user1 = await _userManager.GetUserAsync(HttpContext.User);
                string StaffCode = user1.UserName;

                CheckPayInputByStaff invInput = new CheckPayInputByStaff()
                {
                    CompanyID = inv.CompanyID,
                    OnePayID = inv.OnePayID,
                    StaffCode = user1.UserName
                };
                var a = await _Service.invoiceServices.CheckPayInvoiceByStaff(invInput);
                return Ok(a);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> PayUndo([FromBody] PayCheckModel inv)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Pay {JsonConvert.SerializeObject(inv)}");
            //dynamic rd;
            if (inv.IsAgree)
            {
                var user1 = await _userManager.GetUserAsync(HttpContext.User);
                string StaffCode = user1.UserName;

                CheckPayInputByStaff invInput = new CheckPayInputByStaff()
                {
                    CompanyID = inv.CompanyID,
                    OnePayID = inv.OnePayID,
                    StaffCode = user1.UserName
                };
                var a = await _Service.invoiceServices.CheckPayInvoiceByStaff(invInput);
                return Ok(a);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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
        // Invoice saved by payment
        [HttpPost]
        [Route("[action]/{Page}")]
        public async Task<IActionResult> InvoiceSave(int? Page, [FromBody] InvoiceSaveSearchModel inv)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.WriteLog(_configuration, $"InvoiceSave {JsonConvert.SerializeObject(inv)}", "InvoiceSave");
            var userId = long.Parse(HttpContext.User.Claims.Where(u => u.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value);
            var r = await _Service.iInvoiceSaveServices.InvoceSaveGetListAsync(Page, 10, new InvoiceSaveSearchModelA() { FromDate = inv.FromDate, ToDate = inv.ToDate, CustomerCode = inv.CustomerCode, UserId = userId });
            var c = await _Service.iInvoiceSaveServices.GetCountAsync(new InvoiceSaveSearchModelA() { FromDate = inv.FromDate, ToDate = inv.ToDate, CustomerCode = inv.CustomerCode, UserId = userId });
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            var r = await _Service.iInvoiceSaveServices.DeleteAsync(Id);
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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
        #endregion

        #region Other
        //[HttpPost]
        //[Route("[action]/{Page}")]
        //public async Task<IActionResult> ListAll(int? Page, [FromBody] InvoiceModel invM)
        //{
        //    var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
        //    InvoiceAllInput inv = new InvoiceAllInput()
        //    {
        //        CompanyID = invM.CompanyID,
        //        CustomerCode = invM.CustomerCode,
        //        Page = (Page.HasValue ? Page.Value : 1),
        //        FromDate = (invM.FromDate.HasValue ? invM.FromDate.Value.ToString("MM/dd/yyyy") : ""),
        //        ToDate = (invM.ToDate.HasValue ? invM.ToDate.Value.ToString("MM/dd/yyyy") : ""),
        //        PaymentStatus = (invM.PaymentStatus.HasValue ? invM.PaymentStatus.Value.ToString() : "")
        //    };
        //    string _key = $"ListAll{inv.CompanyID}.{inv.CustomerCode}.{Page}.{inv.Page}.{inv.FromDate}.{inv.ToDate}.{inv.PaymentStatus}".ToMD5Hash();
        //    if (!invoiceCacheKeys.Contains(_key))
        //    {
        //        invoiceCacheKeys.Add(_key);
        //        await _cache.InvoiceSetAsync("InvoiceCacheKeys", invoiceCacheKeys);
        //    }

        //    ResponseOK a = await _cache.GetAsync<ResponseOK>(_key);
        //    if (a == null)
        //    {
        //        a = await _Service.invoiceServices.GetInvoiceAll(inv);
        //        await _cache.InvoiceSetAsync(_key, a);
        //    }
        //    _logger.WriteLog(_configuration, $"ListAll {JsonConvert.SerializeObject(inv)}: {a.UserMessage}", "ListAll");
        //    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
        //    return Ok(a);
        //}



        //[HttpPost]
        //[Route("[action]/{Page}")]
        //public async Task<IActionResult> FindInvoice(int? Page, [FromBody] InvoiceFindModel invM)
        //{
        //    var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
        //    _logger.WriteLog(_configuration, $"FindInvoice {JsonConvert.SerializeObject(invM)}", "FindInvoice");

        //    string _key = $"FindInvoice{invM.FromDate}.{invM.ToDate}.{invM.PaymentStatus}.{Page}".ToMD5Hash();
        //    if (!invoiceCacheKeys.Contains(_key))
        //    {
        //        invoiceCacheKeys.Add(_key);
        //        await _cache.InvoiceSetAsync("InvoiceCacheKeys", invoiceCacheKeys);
        //    }
        //    string _keyCount = $"FindInvoice{invM.FromDate}.{invM.ToDate}.{invM.PaymentStatus}.{Page}.Count".ToMD5Hash();
        //    if (!invoiceCacheKeys.Contains(_keyCount))
        //    {
        //        invoiceCacheKeys.Add(_keyCount);
        //        await _cache.InvoiceSetAsync("InvoiceCacheKeys", invoiceCacheKeys);
        //    }
        //    List<CompanyInvoice> r = await _cache.GetAsync<List<CompanyInvoice>>(_key);
        //    int itemsCount = 0;
        //    if (r == null)
        //    {
        //        r = new List<CompanyInvoice>();
        //        foreach (var companyInfo in companyConfig.Companys)
        //        {
        //            var a = new CompanyInvoice()
        //            {
        //                CompanyCode = companyInfo.Info.CompanyCode,
        //                CompanyId = companyInfo.Info.CompanyId,
        //                CompanyLogo = companyInfo.Info.CompanyLogo,
        //                CompanyNameEn = companyInfo.Info.CompanyNameEn,
        //                CompanyName = companyInfo.Info.CompanyName,
        //                Taxcode = companyInfo.Info.Taxcode,
        //                itemsData = new List<ItemsDatum>()
        //            };
        //            var user1 = await _userManager.GetUserAsync(HttpContext.User);
        //            var claims = await _userManager.GetClaimsAsync(user1);
        //            var contract = claims.Where(c => c.Type == "GetInvoice" && c.Value.StartsWith($"{companyInfo.Info.CompanyId}."));
        //            int CompanyId = companyInfo.Info.CompanyId;
        //            string CustomerCode = "";
        //            foreach (var item in contract)
        //            {
        //                var arr = item.Value.Split(".");
        //                if (arr.Length > 1)
        //                {
        //                    CustomerCode = CustomerCode + "," + arr[1];
        //                }
        //                else
        //                {
        //                    CustomerCode = CustomerCode + "," + arr[0];
        //                }
        //            }
        //            var inv = new InvoiceAllAInput()
        //            {
        //                CompanyID = CompanyId,
        //                CustomerCodeList = CustomerCode,
        //                FromDate = invM.FromDate.HasValue ? invM.FromDate.Value.ToString("MM/dd/yyyy") : "",
        //                ToDate = invM.ToDate.HasValue ? invM.ToDate.Value.ToString("MM/dd/yyyy") : "",
        //                Page = Page.HasValue ? Page.Value : 1,
        //                PaymentStatus = invM.PaymentStatus.HasValue ? invM.PaymentStatus.Value.ToString() : "2"
        //            };
        //            var a2 = await _Service.invoiceServices.GetInvoiceAllA(inv);
        //            if (a2.DataStatus == "00")
        //            {
        //                for (var j = 0; j < a2.ItemsData.Count; j++)
        //                {
        //                    a2.ItemsData[j].CompanyId = CompanyId;
        //                    a2.ItemsData[j].Link = $"https://nuocngoctuan.com/Account/InvoiceView?reservationCode={a2.ItemsData[j].MaSoBiMat}&supplierTaxCode={companyConfig.Companys[inv.CompanyID].Info.Taxcode}& invoiceNo={a2.ItemsData[j].InvSerial}{a2.ItemsData[j].InvNumber}& invoiceType={_configuration["CompanyConfig:InvoiceType"]}";
        //                }
        //                a.itemsData = a2.ItemsData;
        //                itemsCount = itemsCount + int.Parse(a2.Message);
        //            }
        //            r.Add(a);
        //        }
        //        await _cache.InvoiceSetAsync(_key, r);
        //        await _cache.InvoiceSetAsync(_keyCount, itemsCount);
        //    }
        //    else
        //    {
        //        itemsCount = await _cache.GetAsync<int>(_keyCount);
        //    }
        //    ResponseOK a1 = new ResponseOK()
        //    {
        //        Code = 200,
        //        data = new
        //        {
        //            itemsCount = itemsCount,
        //            itemsList = r
        //        },
        //        InternalMessage = LanguageAll.Language.Success,
        //        MoreInfo = LanguageAll.Language.Success,
        //        Status = 1,
        //        UserMessage = LanguageAll.Language.Success
        //    };
        //    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
        //    return Ok(a1);
        //}



        //[HttpGet]
        //[Route("[action]")]
        //public async Task<IActionResult> List()
        //{
        //    var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
        //    string _key = $"List_ALL".ToMD5Hash();
        //    if (!invoiceCacheKeys.Contains(_key))
        //    {
        //        invoiceCacheKeys.Add(_key);
        //        await _cache.InvoiceSetAsync("InvoiceCacheKeys", invoiceCacheKeys);
        //    }
        //    bool IsFound = false;
        //    List<ItemsDatum> r = await _cache.GetAsync<List<ItemsDatum>>(_key);
        //    if (r == null)
        //    {
        //        r = new List<ItemsDatum>();
        //        var user1 = await _userManager.GetUserAsync(HttpContext.User);
        //        var claims = await _userManager.GetClaimsAsync(user1);
        //        foreach (var item in claims.Where(u => u.Type == "GetInvoice"))
        //        {
        //            int CompanyId = 0;
        //            string CustomerCode = "";
        //            var arr = item.Value.Split(".");
        //            if (arr.Length > 1)
        //            {
        //                CompanyId = int.Parse(arr[0]);
        //                CustomerCode = arr[1];
        //            }
        //            else
        //            {
        //                CustomerCode = arr[0];
        //            }
        //            var inv = new InvoiceInput()
        //            {
        //                CompanyID = CompanyId,
        //                CustomerCode = CustomerCode
        //            };
        //            var a = await _Service.invoiceServices.GetInvoiceA(inv);
        //            if (a.DataStatus == "00" || a.DataStatus == "01") IsFound = true;
        //            if (a.DataStatus == "00")
        //            {
        //                for (var j = 0; j < a.ItemsData.Count; j++)
        //                {
        //                    a.ItemsData[j].CompanyId = CompanyId;
        //                    a.ItemsData[j].Link = $"https://nuocngoctuan.com/Account/InvoiceView?reservationCode={a.ItemsData[j].MaSoBiMat}&supplierTaxCode={companyConfig.Companys[inv.CompanyID].Info.Taxcode}& invoiceNo={a.ItemsData[j].InvSerial}{a.ItemsData[j].InvNumber}& invoiceType={_configuration["CompanyConfig:InvoiceType"]}";
        //                }
        //                r.AddRange(a.ItemsData);
        //            }
        //        }
        //        await _cache.InvoiceSetAsync(_key, r);
        //    }
        //    ResponseOK a1;
        //    if (r.Count > 0)
        //        a1 = new ResponseOK()
        //        {
        //            Code = 200,
        //            data = new
        //            {
        //                itemsData = new { invList = r }
        //            },
        //            InternalMessage = LanguageAll.Language.Success,
        //            MoreInfo = LanguageAll.Language.Success,
        //            Status = 1,
        //            UserMessage = LanguageAll.Language.Success
        //        };
        //    else if (IsFound)
        //        a1 = new ResponseOK()
        //        {
        //            Code = 200,
        //            data = new
        //            {
        //                itemsData = new { invList = new List<ItemsDatum>() }
        //            },
        //            InternalMessage = LanguageAll.Language.Success,
        //            MoreInfo = LanguageAll.Language.Success,
        //            Status = 1,
        //            UserMessage = LanguageAll.Language.Success
        //        };
        //    else
        //        a1 = new ResponseOK()
        //        {
        //            Code = 404,
        //            data = null,
        //            InternalMessage = LanguageAll.Language.NotFound,
        //            MoreInfo = LanguageAll.Language.NotFound,
        //            Status = 0,
        //            UserMessage = LanguageAll.Language.NotFound
        //        };
        //    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
        //    return Ok(a1);
        //}



        [HttpGet]
        [Route("[action]")]
        public IActionResult ShowInvoiceCache()
        {
            //var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            if (invoiceCacheKeys.Count > 0)
                return Ok(new ResponseOK()
                {
                    Code = 200,
                    InternalMessage = LanguageAll.Language.Success,
                    MoreInfo = LanguageAll.Language.Success,
                    Status = 1,
                    UserMessage = LanguageAll.Language.Success,
                    data = invoiceCacheKeys
                });
            else
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
        #endregion
    }
}
