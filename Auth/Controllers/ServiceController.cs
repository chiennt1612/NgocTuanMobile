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
using Paygate.OnePay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
    public class ServiceController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<ServiceController> _logger;
        private readonly IAllService _Service;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<ServiceController> _localizer;
        private PaygateInfo paygateInfo;
        private readonly IEmailSender _emailSender;
        public CompanyConfig companyConfig { get; set; }

        public ServiceController(
            IDistributedCache cache,
            ILogger<ServiceController> logger,
            IAllService Service,
            IConfiguration configuration,
            IStringLocalizer<ServiceController> _localizer,
            IEmailSender _emailSender)
        {
            this._logger = logger;
            this._Service = Service;
            this._cache = cache;
            this._configuration = configuration;
            this._localizer = _localizer;
            this._emailSender = _emailSender;
            paygateInfo = this._configuration.GetSection(nameof(PaygateInfo)).Get<PaygateInfo>();
            companyConfig = this._configuration.GetSection(nameof(CompanyConfig)).Get<CompanyConfig>();
            _logger.WriteLog("Starting news page");
        }

        [HttpGet]
        [Route("[action]/{language}")]
        public async Task<IActionResult> List(string language = "Vi")
        {
            List<ServiceModel> r = await _cache.GetAsync<List<ServiceModel>>($"SerrviceList_{language}");//IEnumerable
            if (r == null)
            {
                Expression<Func<Service, bool>> expression = u => u.Id > 10;
                r = (from p in (await _Service.serviceServices.GetManyAsync(expression))
                     select new ServiceModel()
                     {
                         Description = p.Description,
                         Id = p.Id,
                         Img = p.Img,
                         PriceCompany = p.Price1,
                         PricePerson = p.Price,
                         PriceText = p.PriceText,
                         Summary = p.Summary,
                         Title = p.Title
                     }).ToList();
                await _cache.SetAsync<List<ServiceModel>>($"SerrviceList_{language}", r);
            }
            _logger.WriteLog($"Category {language}", $"Category {language}");
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
        public async Task<IActionResult> Details(long Id)
        {
            var a = await _Service.serviceServices.GetByIdAsync(Id);
            var b = new ServiceDetailModel()
            {
                _Detail = a,
                _Related = (await _Service.serviceServices.GetAllAsync()).Where(u => u.Id != Id)
            };
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = b
            });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register(ServiceInputModel contact)
        {
            dynamic rd;
            if (contact.IsAgree)
            {
                var _service = await _Service.serviceServices.GetByIdAsync(contact.ServiceId);
                if (_service != null)
                {
                    var _contact = new Contact()
                    {
                        Address = contact.Address,
                        CompanyName = contact.CompanyName,
                        ContactDate = DateTime.Now,
                        Description = contact.Description,
                        Email = contact.Email,
                        Fullname = contact.Fullname,
                        IsCompany = contact.IsCompany,
                        Mobile = contact.Mobile,
                        Price = (contact.IsCompany ? _service.Price1 : _service.Price),
                        ServiceId = contact.ServiceId,
                        StatusId = 0,
                        UserId = -1,
                        PaymentMethod = (int)contact.PaymentMethod,
                        IsAgree = contact.IsAgree
                    };
                    if (User.Identity.IsAuthenticated)
                    {
                        _contact.Fullname = User.Claims.GetClaimValue("Fullname");
                        _contact.Email = User.Claims.GetClaimValue(ClaimTypes.Email);
                        _contact.Mobile = User.Claims.GetClaimValue(ClaimTypes.Name);
                        _contact.Address = User.Claims.GetClaimValue("Address");
                        _contact.UserId = long.Parse(User.Claims.GetClaimValue(ClaimTypes.NameIdentifier));
                    }
                    if (!string.IsNullOrEmpty(contact.Address)) _contact.Address = contact.Address;
                    if (!string.IsNullOrEmpty(contact.Description)) _contact.Description = contact.Description;
                    if (!string.IsNullOrEmpty(contact.Email)) _contact.Email = contact.Email;
                    if (!string.IsNullOrEmpty(contact.Fullname)) _contact.Fullname = contact.Fullname;
                    if (!string.IsNullOrEmpty(contact.Mobile)) _contact.Mobile = contact.Mobile;

                    var r = await _Service.contactServices.AddAsync(_contact);
                    if (r != null)
                    {
                        _logger.LogInformation($"Send contact is success: {contact.Fullname}");
                        string msg = $"<ul><li><b>{_localizer["Tên đầy đủ"]}:</b> {contact.Fullname}</li><li><b>Email:</b> {contact.Email}</li><li><b>Mobile:</b> {contact.Mobile}</li><li><b>Trạng thái:</b> Chưa thanh toán</li><li>{contact.Description}</li></ul>";

                        switch ((int)contact.PaymentMethod)
                        {
                            case 3:
                                PaymentIn t = new PaymentIn()
                                {
                                    vpc_Amount = (_service.Price).ToString(),
                                    vpc_Customer_Email = contact.Email,
                                    vpc_Customer_Id = (_contact.UserId.HasValue ? _contact.UserId.Value : 0).ToString(),
                                    vpc_Customer_Phone = "",
                                    vpc_MerchTxnRef = (r.Id + Paygate.OnePay.Tools.StartIdOrder).ToString(),
                                    vpc_OrderInfo = $"1_{r.Id}_{_service.Price}",
                                    vpc_SHIP_City = "Han",
                                    vpc_SHIP_Country = "VN",
                                    vpc_SHIP_Provice = "Han",
                                    vpc_SHIP_Street01 = _contact.Address
                                };
                                VPCRequest conn = new VPCRequest(paygateInfo, _logger);
                                var url = conn.CreatePay(HttpContext, paygateInfo, t);
                                _logger.LogInformation(url);
                                rd = new PayModel() { Url = url, order = r };
                                break;
                            default:
                                await _emailSender.SendEmailAsync(contact.Email, _localizer.GetString("v.v Liên hệ Ngọc Tuấn"), msg);
                                rd = new PayModel() { Url = null, order = r };
                                break;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Send contact is fail: {contact.Fullname}");
                        rd = LanguageAll.Language.Fail;
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.ServiceNotFound,
                        MoreInfo = LanguageAll.Language.ServiceNotFound,
                        Status = 0,
                        UserMessage = LanguageAll.Language.ServiceNotFound,
                        data = null
                    });
                }
            }
            else
            {
                ModelState.AddModelError("IsAgree", _localizer.GetString("Bạn chưa chọn đồng ý điều khoản thanh toán!"));
                rd = _localizer.GetString("Bạn chưa chọn đồng ý điều khoản thanh toán!");
            }

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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ChangeContract(ChangeContractInputModel contact)
        {
            dynamic rd;
            if (contact.IsAgree)
            {
                var _service = await _Service.serviceServices.GetByIdAsync(contact.ServiceId);
                if (_service != null)
                {
                    var _contact = new Contact()
                    {
                        Address = contact.Address,
                        CompanyName = contact.CompanyName,
                        ContactDate = DateTime.Now,
                        Description = contact.Description,
                        Email = contact.Email,
                        Fullname = contact.Fullname,
                        IsCompany = contact.IsCompany,
                        Mobile = contact.Mobile,
                        Price = (contact.IsCompany ? _service.Price1 : _service.Price),
                        ServiceId = contact.ServiceId,
                        StatusId = 0,
                        UserId = -1,
                        PaymentMethod = (int)contact.PaymentMethod,
                        IsAgree = contact.IsAgree,
                        Noted = contact.Noted
                    };
                    if (User.Identity.IsAuthenticated)
                    {
                        _contact.Fullname = User.Claims.GetClaimValue("Fullname");
                        _contact.Email = User.Claims.GetClaimValue(ClaimTypes.Email);
                        _contact.Mobile = User.Claims.GetClaimValue(ClaimTypes.Name);
                        _contact.Address = User.Claims.GetClaimValue("Address");
                        _contact.UserId = long.Parse(User.Claims.GetClaimValue(ClaimTypes.NameIdentifier));
                    }
                    if (!string.IsNullOrEmpty(contact.Address)) _contact.Address = contact.Address;
                    if (!string.IsNullOrEmpty(contact.Description)) _contact.Description = contact.Description;
                    if (!string.IsNullOrEmpty(contact.Email)) _contact.Email = contact.Email;
                    if (!string.IsNullOrEmpty(contact.Fullname)) _contact.Fullname = contact.Fullname;
                    if (!string.IsNullOrEmpty(contact.Mobile)) _contact.Mobile = contact.Mobile;

                    var r = await _Service.contactServices.AddAsync(_contact);
                    if (r != null)
                    {
                        _logger.LogInformation($"Send contact is success: {contact.Fullname}");
                        string msg = $"<ul><li><b>{_localizer["Tên đầy đủ"]}:</b> {contact.Fullname}</li><li><b>Email:</b> {contact.Email}</li><li><b>Mobile:</b> {contact.Mobile}</li><li><b>Trạng thái:</b> Chưa thanh toán</li><li>{contact.Description}</li></ul>";

                        switch ((int)contact.PaymentMethod)
                        {
                            case 3:
                                PaymentIn t = new PaymentIn()
                                {
                                    vpc_Amount = (_service.Price).ToString(),
                                    vpc_Customer_Email = contact.Email,
                                    vpc_Customer_Id = (_contact.UserId.HasValue ? _contact.UserId.Value : 0).ToString(),
                                    vpc_Customer_Phone = "",
                                    vpc_MerchTxnRef = (r.Id + Paygate.OnePay.Tools.StartIdOrder).ToString(),
                                    vpc_OrderInfo = $"1_{r.Id}_{_service.Price}",
                                    vpc_SHIP_City = "Han",
                                    vpc_SHIP_Country = "VN",
                                    vpc_SHIP_Provice = "Han",
                                    vpc_SHIP_Street01 = _contact.Address
                                };
                                VPCRequest conn = new VPCRequest(paygateInfo, _logger);
                                var url = conn.CreatePay(HttpContext, paygateInfo, t);
                                _logger.LogInformation(url);
                                rd = new PayModel() { Url = url, order = r };
                                break;
                            default:
                                await _emailSender.SendEmailAsync(contact.Email, _localizer.GetString("v.v Liên hệ Ngọc Tuấn"), msg);
                                rd = new PayModel() { Url = null, order = r };
                                break;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Send contact is fail: {contact.Fullname}");
                        rd = LanguageAll.Language.Fail;
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.ServiceNotFound,
                        MoreInfo = LanguageAll.Language.ServiceNotFound,
                        Status = 0,
                        UserMessage = LanguageAll.Language.ServiceNotFound,
                        data = null
                    });
                }
            }
            else
            {
                ModelState.AddModelError("IsAgree", _localizer.GetString("Bạn chưa chọn đồng ý điều khoản thanh toán!"));
                rd = _localizer.GetString("Bạn chưa chọn đồng ý điều khoản thanh toán!");
            }

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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ChangePosition(ChangePositionInputModel contact)
        {
            dynamic rd;
            if (contact.IsAgree)
            {
                var _service = await _Service.serviceServices.GetByIdAsync(contact.ServiceId);
                if (_service != null)
                {
                    string Img = "";
                    if (!string.IsNullOrEmpty(contact.Img))
                    {
                        Img = Upload(contact.Img);
                    }
                    string Img2 = "";
                    if (!string.IsNullOrEmpty(contact.Img2))
                    {
                        Img2 = Upload(contact.Img2);
                    }
                    var _contact = new Contact()
                    {
                        Address = contact.Address,
                        CompanyName = contact.CompanyName,
                        ContactDate = DateTime.Now,
                        Description = contact.Description,
                        Email = contact.Email,
                        Fullname = contact.Fullname,
                        IsCompany = contact.IsCompany,
                        Mobile = contact.Mobile,
                        Price = (contact.IsCompany ? _service.Price1 : _service.Price),
                        ServiceId = contact.ServiceId,
                        StatusId = 0,
                        UserId = -1,
                        PaymentMethod = (int)contact.PaymentMethod,
                        IsAgree = contact.IsAgree,
                        Noted = contact.Noted,
                        Noted2 = contact.Noted2,
                        Img = Img,
                        Img2 = Img2
                    };
                    if (User.Identity.IsAuthenticated)
                    {
                        _contact.Fullname = User.Claims.GetClaimValue("Fullname");
                        _contact.Email = User.Claims.GetClaimValue(ClaimTypes.Email);
                        _contact.Mobile = User.Claims.GetClaimValue(ClaimTypes.Name);
                        _contact.Address = User.Claims.GetClaimValue("Address");
                        _contact.UserId = long.Parse(User.Claims.GetClaimValue(ClaimTypes.NameIdentifier));
                    }
                    if (!string.IsNullOrEmpty(contact.Address)) _contact.Address = contact.Address;
                    if (!string.IsNullOrEmpty(contact.Description)) _contact.Description = contact.Description;
                    if (!string.IsNullOrEmpty(contact.Email)) _contact.Email = contact.Email;
                    if (!string.IsNullOrEmpty(contact.Fullname)) _contact.Fullname = contact.Fullname;
                    if (!string.IsNullOrEmpty(contact.Mobile)) _contact.Mobile = contact.Mobile;

                    var r = await _Service.contactServices.AddAsync(_contact);
                    if (r != null)
                    {
                        _logger.LogInformation($"Send contact is success: {contact.Fullname}");
                        string msg = $"<ul><li><b>{_localizer["Tên đầy đủ"]}:</b> {contact.Fullname}</li><li><b>Email:</b> {contact.Email}</li><li><b>Mobile:</b> {contact.Mobile}</li><li><b>Trạng thái:</b> Chưa thanh toán</li><li>{contact.Description}</li></ul>";

                        switch ((int)contact.PaymentMethod)
                        {
                            case 3:
                                PaymentIn t = new PaymentIn()
                                {
                                    vpc_Amount = (_service.Price).ToString(),
                                    vpc_Customer_Email = contact.Email,
                                    vpc_Customer_Id = (_contact.UserId.HasValue ? _contact.UserId.Value : 0).ToString(),
                                    vpc_Customer_Phone = "",
                                    vpc_MerchTxnRef = (r.Id + Paygate.OnePay.Tools.StartIdOrder).ToString(),
                                    vpc_OrderInfo = $"1_{r.Id}_{_service.Price}",
                                    vpc_SHIP_City = "Han",
                                    vpc_SHIP_Country = "VN",
                                    vpc_SHIP_Provice = "Han",
                                    vpc_SHIP_Street01 = _contact.Address
                                };
                                VPCRequest conn = new VPCRequest(paygateInfo, _logger);
                                var url = conn.CreatePay(HttpContext, paygateInfo, t);
                                _logger.LogInformation(url);
                                rd = new PayModel() { Url = url, order = r };
                                break;
                            default:
                                await _emailSender.SendEmailAsync(contact.Email, _localizer.GetString("v.v Liên hệ Ngọc Tuấn"), msg);
                                rd = new PayModel() { Url = null, order = r };
                                break;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Send contact is fail: {contact.Fullname}");
                        rd = LanguageAll.Language.Fail;
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.ServiceNotFound,
                        MoreInfo = LanguageAll.Language.ServiceNotFound,
                        Status = 0,
                        UserMessage = LanguageAll.Language.ServiceNotFound,
                        data = null
                    });
                }
            }
            else
            {
                ModelState.AddModelError("IsAgree", _localizer.GetString("Bạn chưa chọn đồng ý điều khoản thanh toán!"));
                rd = _localizer.GetString("Bạn chưa chọn đồng ý điều khoản thanh toán!");
            }

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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ChangePrice(ChangePriceInputModel contact)
        {
            dynamic rd;
            if (contact.IsAgree)
            {
                var _service = await _Service.serviceServices.GetByIdAsync(contact.ServiceId);
                if (_service != null)
                {
                    string Img = "";
                    if (!string.IsNullOrEmpty(contact.Img))
                    {
                        Img = Upload(contact.Img);
                    }
                    var _contact = new Contact()
                    {
                        Address = contact.Address,
                        CompanyName = contact.CompanyName,
                        ContactDate = DateTime.Now,
                        Description = contact.Description,
                        Email = contact.Email,
                        Fullname = contact.Fullname,
                        IsCompany = contact.IsCompany,
                        Mobile = contact.Mobile,
                        Price = (contact.IsCompany ? _service.Price1 : _service.Price),
                        ServiceId = contact.ServiceId,
                        StatusId = 0,
                        UserId = -1,
                        PaymentMethod = (int)contact.PaymentMethod,
                        IsAgree = contact.IsAgree,
                        Img = Img
                    };
                    if (User.Identity.IsAuthenticated)
                    {
                        _contact.Fullname = User.Claims.GetClaimValue("Fullname");
                        _contact.Email = User.Claims.GetClaimValue(ClaimTypes.Email);
                        _contact.Mobile = User.Claims.GetClaimValue(ClaimTypes.Name);
                        _contact.Address = User.Claims.GetClaimValue("Address");
                        _contact.UserId = long.Parse(User.Claims.GetClaimValue(ClaimTypes.NameIdentifier));
                    }
                    if (!string.IsNullOrEmpty(contact.Address)) _contact.Address = contact.Address;
                    if (!string.IsNullOrEmpty(contact.Description)) _contact.Description = contact.Description;
                    if (!string.IsNullOrEmpty(contact.Email)) _contact.Email = contact.Email;
                    if (!string.IsNullOrEmpty(contact.Fullname)) _contact.Fullname = contact.Fullname;
                    if (!string.IsNullOrEmpty(contact.Mobile)) _contact.Mobile = contact.Mobile;

                    var r = await _Service.contactServices.AddAsync(_contact);
                    if (r != null)
                    {
                        _logger.LogInformation($"Send contact is success: {contact.Fullname}");
                        string msg = $"<ul><li><b>{_localizer["Tên đầy đủ"]}:</b> {contact.Fullname}</li><li><b>Email:</b> {contact.Email}</li><li><b>Mobile:</b> {contact.Mobile}</li><li><b>Trạng thái:</b> Chưa thanh toán</li><li>{contact.Description}</li></ul>";

                        switch ((int)contact.PaymentMethod)
                        {
                            case 3:
                                PaymentIn t = new PaymentIn()
                                {
                                    vpc_Amount = (_service.Price).ToString(),
                                    vpc_Customer_Email = contact.Email,
                                    vpc_Customer_Id = (_contact.UserId.HasValue ? _contact.UserId.Value : 0).ToString(),
                                    vpc_Customer_Phone = "",
                                    vpc_MerchTxnRef = (r.Id + Paygate.OnePay.Tools.StartIdOrder).ToString(),
                                    vpc_OrderInfo = $"1_{r.Id}_{_service.Price}",
                                    vpc_SHIP_City = "Han",
                                    vpc_SHIP_Country = "VN",
                                    vpc_SHIP_Provice = "Han",
                                    vpc_SHIP_Street01 = _contact.Address
                                };
                                VPCRequest conn = new VPCRequest(paygateInfo, _logger);
                                var url = conn.CreatePay(HttpContext, paygateInfo, t);
                                _logger.LogInformation(url);
                                rd = new PayModel() { Url = url, order = r };
                                break;
                            default:
                                await _emailSender.SendEmailAsync(contact.Email, _localizer.GetString("v.v Liên hệ Ngọc Tuấn"), msg);
                                rd = new PayModel() { Url = null, order = r };
                                break;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Send contact is fail: {contact.Fullname}");
                        rd = LanguageAll.Language.Fail;
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.ServiceNotFound,
                        MoreInfo = LanguageAll.Language.ServiceNotFound,
                        Status = 0,
                        UserMessage = LanguageAll.Language.ServiceNotFound,
                        data = null
                    });
                }
            }
            else
            {
                ModelState.AddModelError("IsAgree", _localizer.GetString("Bạn chưa chọn đồng ý điều khoản thanh toán!"));
                rd = _localizer.GetString("Bạn chưa chọn đồng ý điều khoản thanh toán!");
            }

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

        private string Upload(string file)
        {
            // Check Avatar and save to admin.nuocngoctuan.com                
            if (!string.IsNullOrEmpty(file))
            {
                _logger.WriteLog($"1. Upload {file}");
                var username = User.Claims.Where(u => u.Type == ClaimTypes.Name).FirstOrDefault()?.Value;
                if (string.IsNullOrEmpty(username)) username = "admin";
                string path = companyConfig.AvatarFolder + @"/" + username;
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                _logger.WriteLog($"2. Upload {path}/{fileName}");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                if (System.IO.File.Exists(path + @"/" + fileName)) System.IO.File.Delete(path + @"/" + fileName);
                System.IO.File.WriteAllBytes(path + @"/" + fileName, Convert.FromBase64String(file));
                file = @"https://admin.nuocngoctuan.com/Upload/Avatar/" + fileName;
                _logger.WriteLog($"3. Upload {file}");
            }
            return file;
        }
    }
}
