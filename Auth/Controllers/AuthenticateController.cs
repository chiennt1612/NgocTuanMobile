using Auth.Helper;
using Auth.Models;
using Auth.Repository.Interfaces;
using Auth.Services;
using Auth.Services.Interfaces;
using EntityFramework.API.Entities;
using EntityFramework.API.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SMSGetway;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils;
using Utils.ExceptionHandling;
using Utils.Models;
using Utils.Tokens.Interfaces;

namespace Auth.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v1.0/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json", "application/problem+json")]
    //[ValidateModel]
    //[ValidateAntiForgeryToken]
    public class AuthenticateController : ControllerBase
    {
        private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory;
        private readonly AppUserManager _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ISMSVietel _smsVietel;
        private readonly ITokenCreationService _jwtToken;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly LoginConfiguration _loginConfiguration;
        private readonly IAllService _iInvoiceServices;
        private readonly IUserDeviceRepository _iUserDeviceRepository;
        private CompanyConfig companyConfig { get; set; }
        private HtmlXSSFilter htmlXSS { get; set; }
        private FireBaseConfig fireBaseConfig { get; set; }
        private FireBaseAPIConfig fireBaseAPIConfig { get; set; }

        public AuthenticateController(
            ILogger<AuthenticateController> logger,
            AppUserManager userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration,
            ISMSVietel smsVietel,
            ITokenCreationService jwtToken,
            IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
            LoginConfiguration loginConfiguration, IAllService iInvoiceServices,
            IUserDeviceRepository iUserDeviceRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _smsVietel = smsVietel;
            _logger = logger;
            _jwtToken = jwtToken;
            _signInManager = signInManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _loginConfiguration = loginConfiguration;
            _iInvoiceServices = iInvoiceServices;
            _iUserDeviceRepository = iUserDeviceRepository;
            companyConfig = this._configuration.GetSection(nameof(CompanyConfig)).Get<CompanyConfig>();
            htmlXSS = this._configuration.GetSection(nameof(HtmlXSSFilter)).Get<HtmlXSSFilter>();
            fireBaseConfig = this._configuration.GetSection(nameof(FireBaseConfig)).Get<FireBaseConfig>();
            fireBaseAPIConfig = this._configuration.GetSection(nameof(FireBaseAPIConfig)).Get<FireBaseAPIConfig>();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Login. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                bool IsExitst = false;
                _logger.LogInformation($"Finding username: {model.Username}");
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    IsExitst = true;
                    _logger.LogInformation($"Found: {model.Username}");
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, _configuration["Password:Default"], bool.Parse(_configuration["Password:RememberLogin"]), lockoutOnFailure: true);
                    if (result.Succeeded || result.RequiresTwoFactor)
                    {
                        _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                        return await SendSMSOTP(user, model);
                    }
                    else if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                        return StatusCode(StatusCodes.Status200OK,
                                    new ResponseBase(LanguageAll.Language.Fail, $"{model.Username}: {LanguageAll.Language.AccountLockout}!", $"{model.Username}: {LanguageAll.Language.AccountLockout}!", 0, 400));
                    }
                }

                if (!IsExitst)
                {
                    for (var i = 0; i < companyConfig.Companys.Count; i++)
                    {
                        var inv = new EVNCodeInput()
                        {
                            CompanyID = companyConfig.Companys[i].Info.CompanyId,
                            EVNCode = model.Username
                        };
                        var a = await _iInvoiceServices.invoiceServices.getCustomerInfo(inv);
                        if (a.DataStatus == "00")
                        {
                            if (a.ItemsData[0].Email.IsValidEmail())
                                user = new()
                                {
                                    SecurityStamp = Guid.NewGuid().ToString(),
                                    UserName = a.ItemsData[0].Mobile,
                                    Email = a.ItemsData[0].Email,
                                    PhoneNumber = a.ItemsData[0].Mobile,
                                    TwoFactorEnabled = true,
                                };
                            else
                                user = new()
                                {
                                    SecurityStamp = Guid.NewGuid().ToString(),
                                    UserName = a.ItemsData[0].Mobile,
                                    PhoneNumber = a.ItemsData[0].Mobile,
                                    TwoFactorEnabled = true,
                                };
                            var result = await _userManager.CreateAsync(user, _configuration["Password:Default"]);
                            if (result.Succeeded)
                            {
                                var userExists = await _userManager.FindByNameAsync(a.ItemsData[0].Mobile);
                                // Add role customer
                                await _userManager.AddToRoleAsync(userExists, "Customer");
                                // Update address
                                var a1 = await _userManager.GetClaimsAsync(userExists);
                                if (!String.IsNullOrEmpty(a.ItemsData[0].Address))
                                {
                                    await _userManager.AddClaimAsync(userExists, new Claim("Address", a.ItemsData[0].Address));
                                }
                                if (!String.IsNullOrEmpty(a.ItemsData[0].WaterIndexCode))
                                {
                                    await _userManager.AddClaimAsync(userExists, new Claim("WaterCode", a.ItemsData[0].WaterIndexCode));
                                }
                                if (!String.IsNullOrEmpty(a.ItemsData[0].TaxCode))
                                {
                                    await _userManager.AddClaimAsync(userExists, new Claim("TaxCode", a.ItemsData[0].TaxCode));
                                }
                                await _userManager.AddClaimAsync(userExists, new Claim("Fullname", a.ItemsData[0].CustomerName));

                                foreach (var customerCode in a.ItemsData)
                                {
                                    var _claim = new Claim("GetInvoice", $"{inv.CompanyID}.{customerCode.CustomerCode}");
                                    await _userManager.AddClaimAsync(userExists, _claim); //
                                    await _iInvoiceServices.contractServices.AddAsync(new EntityFramework.API.Entities.Contract()
                                    {
                                        Address = customerCode.Address,
                                        CompanyId = inv.CompanyID,
                                        CustomerCode = customerCode.CustomerCode,
                                        CustomerName = customerCode.CustomerName,
                                        CustomerType = customerCode.CustomerType,
                                        Email = customerCode.Email,
                                        Mobile = customerCode.Mobile,
                                        TaxCode = customerCode.TaxCode,
                                        UserId = userExists.Id,
                                        WaterIndexCode = customerCode.WaterIndexCode
                                    });
                                }

                                // Send SMS
                                //userExists.TotalOTP = 1;
                                //userExists.OTPSendTime = DateTime.Now;
                                //await _userManager.UpdateAsync(userExists);
                                //var code = await _userManager.GenerateChangePhoneNumberTokenAsync(userExists, a.ItemsData.Mobile);
                                //var message = $"Your security code is: {code}";
                                //_smsVietel.SendSMS(await _userManager.GetPhoneNumberAsync(userExists), message);
                                //return Ok(new ResponseBase(LanguageAll.Language.VerifyOTP, $"{a.ItemsData.Mobile}: {LanguageAll.Language.VerifyOTP}!", $"{a.ItemsData.Mobile}: {LanguageAll.Language.VerifyOTP}!", 1, 200));
                                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                                return await SendSMSOTP(userExists, new LoginModel() { Username = userExists.UserName });
                            }
                        }
                    }
                }

                if (!IsExitst)
                {
                    _logger.LogInformation($"Not found: {model.Username}");
                    user = new()
                    {
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = model.Username,
                        PhoneNumber = model.Username,
                        TwoFactorEnabled = true,
                    };
                    var result = await _userManager.CreateAsync(user, _configuration["Password:Default"]);
                    if (result.Succeeded)
                    {
                        var userExists = await _userManager.FindByNameAsync(model.Username);
                        // Add role customer
                        await _userManager.AddToRoleAsync(userExists, "Customer");
                        await _userManager.AddClaimAsync(userExists, new Claim("Fullname", model.Username));
                        _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                        return await SendSMSOTP(userExists, new LoginModel() { Username = userExists.UserName });
                    }
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return StatusCode(StatusCodes.Status200OK, new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.NotFound,
                        MoreInfo = LanguageAll.Language.NotFound,
                        Status = 0,
                        UserMessage = LanguageAll.Language.NotFound,
                        data = null
                    });
                }
            }

            _logger.LogInformation($"Not found: {model.Username}");
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.Fail, $"{model.Username}: {LanguageAll.Language.NotFound}!", LanguageAll.Language.Fail));
        }

        //[HttpPost]
        //[Route("verifyCode")]
        //public async Task<IActionResult> VerifyCode([FromBody] OTPModel model)
        //{
        //    _logger.LogInformation($"ModelState: {ModelState.IsValid}");
        //    if (ModelState.IsValid)
        //    {
        //        _logger.LogInformation($"Validating OTP: {model.Code}");
        //        var result = await _signInManager.TwoFactorSignInAsync(_configuration["Password:Provider"], model.Code, bool.Parse(_configuration["Password:RememberLogin"]), bool.Parse(_configuration["Password:RememberBrowser"]));
        //        if (result.Succeeded)
        //        {
        //            var user = await GetCurrentUserAsync();
        //            return await LoginOK(user);
        //        }
        //        if (result.IsLockedOut)
        //        {
        //            _logger.LogWarning("User account locked out.");
        //            return StatusCode(StatusCodes.Status200OK,
        //                new ResponseBase("Login fail!", $"User account locked out!", "Login fail!"));
        //        }
        //    }
        //    _logger.LogWarning("Invalid code.");
        //    return StatusCode(StatusCodes.Status200OK,
        //        new ResponseBase("Invalid code.", $"Invalid code.", "Login fail!"));
        //}

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Register. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                if (_smsVietel.FormatMobile(model.Username) != "")
                {
                    LoginModel loginModel = new LoginModel()
                    {
                        Username = model.Username
                    };
                    var userExists = await _userManager.FindByNameAsync(model.Username);
                    if (userExists != null)
                    {
                        _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                        return await SendSMSOTP(userExists, loginModel);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(model.Fullname))
                        {
                            AppUser user;
                            if (model.Email.IsValidEmail())
                                user = new()
                                {
                                    SecurityStamp = Guid.NewGuid().ToString(),
                                    UserName = model.Username,
                                    Email = model.Email,
                                    PhoneNumber = model.Username,
                                    TwoFactorEnabled = true,
                                };
                            else
                                user = new()
                                {
                                    SecurityStamp = Guid.NewGuid().ToString(),
                                    UserName = model.Username,
                                    PhoneNumber = model.Username,
                                    TwoFactorEnabled = true,
                                };
                            var result = await _userManager.CreateAsync(user, _configuration["Password:Default"]);
                            if (result.Succeeded)
                            {
                                userExists = await _userManager.FindByNameAsync(model.Username);
                                // Add role customer
                                await _userManager.AddToRoleAsync(userExists, "Customer");
                                // Update address
                                var a = await _userManager.GetClaimsAsync(userExists);
                                await _userManager.AddClaimAsync(userExists, new Claim("Fullname", model.Fullname));
                                if (!String.IsNullOrEmpty(model.Address))
                                {
                                    if (String.IsNullOrEmpty(a.Where(u => u.Type == "Address").FirstOrDefault()?.Value))
                                    {
                                        await _userManager.AddClaimAsync(userExists, new Claim("Address", model.Address));
                                    }
                                    else
                                    {
                                        await _userManager.ReplaceClaimAsync(userExists, a.Where(u => u.Type == "Address").FirstOrDefault(), new Claim("Address", model.Address));
                                    }
                                }
                                // Send SMS
                                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                                return await SendSMSOTP(userExists, new LoginModel() { Username = userExists.UserName });
                            }
                        }
                    }
                }
                else
                {
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return StatusCode(StatusCodes.Status200OK, new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.FormatPhoneNumberFail,
                        MoreInfo = LanguageAll.Language.FormatPhoneNumberFail,
                        Status = 0,
                        UserMessage = LanguageAll.Language.FormatPhoneNumberFail,
                        data = null
                    });
                }
            }
            _logger.LogError($"{model.Username}: {LanguageAll.Language.UserCreateFail}");
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.UserCreateFail, LanguageAll.Language.UserCreateFail, LanguageAll.Language.UserCreateFail));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> VerifyPhoneNumber([FromBody] OTPModel model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"VerifyPhoneNumber. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                //var user = await GetCurrentUserAsync();
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    string PhoneNumber = user.UserName;
                    if (_loginConfiguration.MobileTest.Contains(PhoneNumber))
                    {
                        _logger.LogInformation($"Validating OTP: {model.Code} with phone number: {PhoneNumber}; [TEST]");
                        if (model.Code == _loginConfiguration.OTPTest)
                        {
                            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                            return await LoginOK(model.DeviceId, user);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Validating OTP: {model.Code} with phone number: {PhoneNumber}");
                        var result = await _userManager.ChangePhoneNumberAsync(user, PhoneNumber, model.Code);
                        if (result.Succeeded)
                        {
                            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                            return await LoginOK(model.DeviceId, user);
                        }
                    }
                }
                else
                {
                    _logger.LogInformation($"Not found: {model.Username}");
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return StatusCode(StatusCodes.Status200OK, new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.NotFound,
                        MoreInfo = LanguageAll.Language.NotFound,
                        Status = 0,
                        UserMessage = LanguageAll.Language.NotFound,
                        data = null
                    });
                }
            }
            _logger.LogWarning("Invalid code.");
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.OTPInvalid, LanguageAll.Language.OTPInvalid, LanguageAll.Language.OTPInvalid));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RenewToken([FromBody] RefreshTokenModel model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"RenewToken. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                var encodedString = HttpContext.Request.Headers["Authorization"];
                var a = _jwtToken.ValidateToken(encodedString);
                if (a == null || a == default)
                {
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return StatusCode(StatusCodes.Status200OK,
                        new ResponseBase(LanguageAll.Language.Unauthorized, LanguageAll.Language.Unauthorized, LanguageAll.Language.Unauthorized, 0, 401));
                }
                var user = await GetCurrentUserAsync(a);
                string RefreshToken = "";
                DateTime RefreshTokenExpiryTime = DateTime.Now.AddDays(1);
                if (!String.IsNullOrEmpty(model.DeviceId))
                {
                    //Expression<Func<AppUserDevice, bool>> sqlWhere = u => (u.DeviceID == model.DeviceId && u.Username == user.UserName);
                    Expression<Func<AppUserDevice, bool>> sqlWhere = u => (u.RefreshToken == model.RefreshToken && u.Username == user.UserName);
                    var UserByDevice = await _iUserDeviceRepository.GetAsync(sqlWhere);                  
                    if (UserByDevice != default)
                    {
                        model.DeviceId = UserByDevice.DeviceID;
                        RefreshToken = UserByDevice.RefreshToken;
                        if (UserByDevice.RefreshTokenExpiryTime.HasValue)
                            RefreshTokenExpiryTime = UserByDevice.RefreshTokenExpiryTime.Value;
                        _logger.LogInformation($"RenewToken RefreshToken/UserByDevice: {RefreshToken}");
                    }
                }
                if (String.IsNullOrEmpty(RefreshToken) && !String.IsNullOrEmpty(user.RefreshToken))
                {
                    RefreshToken = user.RefreshToken;
                    if (user.RefreshTokenExpiryTime.HasValue)
                        RefreshTokenExpiryTime = user.RefreshTokenExpiryTime.Value;
                    _logger.LogInformation($"RenewToken RefreshToken/user: {RefreshToken}");
                }
                if (!String.IsNullOrEmpty(RefreshToken))
                {
                    if (RefreshToken == model.RefreshToken && RefreshTokenExpiryTime >= DateTime.Now)
                    {
                        _logger.LogInformation($"RenewToken {user.UserName} TRUE model: {JsonConvert.SerializeObject(model)}");
                        _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                        return await LoginOK(model.DeviceId, user);
                    }
                }
                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                return StatusCode(StatusCodes.Status200OK,
                    new ResponseBase(LanguageAll.Language.RefreshTokenInvalid, LanguageAll.Language.RefreshTokenInvalid, LanguageAll.Language.RefreshTokenInvalid, 0, 400));
            }

            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.RenewTokenFail, LanguageAll.Language.RenewTokenFail, LanguageAll.Language.RenewTokenFail));
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Revoke()
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            var user = await GetCurrentUserAsync(HttpContext.User);

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
            Expression<Func<AppUserDevice, bool>> sqlWhere = u => (u.Username == user.UserName);
            var devices = (await _iUserDeviceRepository.GetManyAsync(sqlWhere)).ToList();
            for (int i = 0; i < devices.Count; i++)
            {
                devices[i].RefreshToken = null;
            }
            await _iUserDeviceRepository.UpdateMany(devices);
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return Ok(new ResponseBase(LanguageAll.Language.Success, LanguageAll.Language.Success, LanguageAll.Language.Success, 1, 200));
        }

        //[Authorize]
        //[HttpPost]
        //[Route("revoke-all")]
        //public async Task<IActionResult> RevokeAll()
        //{
        //    var users = _userManager.Users.ToList();
        //    foreach (var user in users)
        //    {
        //        user.RefreshToken = null;
        //        await _userManager.UpdateAsync(user);
        //    }

        //    return Ok(new ResponseBase("Invalid code.", $"Invalid code.", "Login fail!"));
        //}

        private async Task<IActionResult> LoginOK(string DeviceId, AppUser user)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"LoginOK -> Logined: {user.UserName}; DeviceId: {DeviceId}");
            ClaimsPrincipal userClaims = await _userClaimsPrincipalFactory.CreateAsync(user);
            List<Claim> claims = userClaims.Claims.ToList();
            //var userRoles = await _userManager.GetRolesAsync(user);
            //foreach (var userRole in userRoles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, userRole));
            //}
            //var userClaim = await _userManager.GetClaimsAsync(user);
            ////claims.AddRange(userClaim);
            //foreach (var claim in userClaim)
            //{
            //    claims.Add(claim);
            //}
            claims.Add(new Claim("DeviceId", DeviceId));
            var _IsGetNotice = new Claim("IsGetNotice", "0");
            claims.Add(_IsGetNotice);
            claims.Add(new Claim("username", user.UserName));
            claims.Add(new Claim("aud", _configuration["JWT:ValidAudience"]));
            //claims.Add(new Claim("oldid", user.OldId.ToString()));

            var token = await _jwtToken.CreateTokenAsync(_jwtToken.CreateAccessTokenAsync(claims));
            var refreshToken = _jwtToken.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
            _logger.LogInformation($"RefreshToken: {refreshToken}\nToken: {token}\n");
            await _userManager.UpdateAsync(user);
            if (!String.IsNullOrEmpty(DeviceId))
            {
                Expression<Func<AppUserDevice, bool>> sqlWhere = u => (u.DeviceID == DeviceId);
                var UserByDevice = await _iUserDeviceRepository.GetAsync(sqlWhere);
                if (UserByDevice != default)
                {
                    _logger.LogInformation($"Found device {DeviceId}");
                    //if (UserByDevice.Username == user.UserName)
                    //{
                        claims.Remove(_IsGetNotice);
                        claims.Add(new Claim("IsGetNotice", UserByDevice.IsGetNotice ? "1" : "0"));
                        _logger.LogInformation($"Found device {DeviceId}/{user.UserName}");
                        UserByDevice.Username = user.UserName;
                        UserByDevice.RefreshToken = user.RefreshToken;
                        UserByDevice.RefreshTokenExpiryTime = user.RefreshTokenExpiryTime;
                        await _iUserDeviceRepository.Update(UserByDevice);
                    //}
                }
                else
                {
                    _logger.LogInformation($"Addnew device {DeviceId}/{user.UserName}");
                    UserByDevice = new AppUserDevice()
                    {
                        DeviceID = DeviceId,
                        IsGetNotice = false,
                        OS = OSType.Android,
                        Username = user.UserName,
                        RefreshToken = user.RefreshToken,
                        RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
                    };
                    await _iUserDeviceRepository.AddAsync(UserByDevice);
                }
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return Ok(new LoginSuccessModel
            {
                Status = 1,
                UserMessage = "Login success!",
                InternalMessage = $"{user.UserName}: Login success!",
                Code = 200,
                MoreInfo = $"Login success!",
                data = new LoginData
                {
                    Token = token,
                    RefreshToken = refreshToken
                }
            });
        }

        private Task<AppUser> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            //_logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return _userManager.GetUserAsync(user);
        }

        private async Task<IActionResult> SendSMSOTP(AppUser user, LoginModel model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            int IsNotAllowSend = 0;
            if (user.OTPSendTime.HasValue)
            {
                TimeSpan span = DateTime.Now.Subtract(user.OTPSendTime.Value);
                if ((int)span.TotalDays == 0)
                {
                    if ((int)span.TotalMinutes < _loginConfiguration.OTPTimeLife)
                    {
                        IsNotAllowSend = 2;
                    }
                    else if (user.TotalOTP >= _loginConfiguration.OTPLimitedOnDay)
                    {
                        IsNotAllowSend = 3;
                    }
                    else IsNotAllowSend = 1;
                }
            }
            // For test
            if (_loginConfiguration.MobileTest.Contains(user.UserName)) IsNotAllowSend = 4;
            switch (IsNotAllowSend)
            {
                case 2:
                    _logger.LogWarning("You needs request OTP after 3 minutes.");
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return StatusCode(StatusCodes.Status200OK,
                        new ResponseBase(LanguageAll.Language.Fail, $"{model.Username}: {LanguageAll.Language.OTPWait}!", $"{model.Username}: {LanguageAll.Language.OTPWait}!"));
                case 3:
                    _logger.LogWarning("You request too more OTP on this day.");
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return StatusCode(StatusCodes.Status200OK,
                        new ResponseBase(LanguageAll.Language.Fail, $"{model.Username}: {LanguageAll.Language.OTPLimited}!", $"{model.Username}: {LanguageAll.Language.OTPLimited}!"));
                case 4:
                    user.TotalOTP = user.TotalOTP + 1;
                    user.OTPSendTime = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return StatusCode(StatusCodes.Status200OK, new ResponseOK()
                    {
                        Code = 200,
                        InternalMessage = LanguageAll.Language.VerifyOTP,
                        MoreInfo = LanguageAll.Language.VerifyOTP,
                        Status = 1,
                        UserMessage = LanguageAll.Language.VerifyOTP,
                        data = model
                    });
                //Ok(new ResponseBase(LanguageAll.Language.VerifyOTP, $"{model.Username}: {LanguageAll.Language.VerifyOTP}!", $"{model.Username}: {LanguageAll.Language.VerifyOTP}!", 1, 200));
                default:
                    //var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
                    //var message = $"Your security code is: {code}";
                    //_smsVietel.SendSMS(await _userManager.GetPhoneNumberAsync(user), message);
                    if (IsNotAllowSend == 0)
                        user.TotalOTP = 1;
                    else
                        user.TotalOTP = user.TotalOTP + 1;
                    user.OTPSendTime = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                    var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.Username);
                    var message = _loginConfiguration.OTPSMSContent.Replace("{OTPCODE}", code);
                    _smsVietel.SendSMS(await _userManager.GetPhoneNumberAsync(user), message);
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return StatusCode(StatusCodes.Status200OK, new ResponseOK()
                    {
                        Code = 200,
                        InternalMessage = LanguageAll.Language.VerifyOTP,
                        MoreInfo = LanguageAll.Language.VerifyOTP,
                        Status = 1,
                        UserMessage = LanguageAll.Language.VerifyOTP,
                        data = model
                    });
                    //Ok(new ResponseBase(LanguageAll.Language.VerifyOTP, $"{model.Username}: {LanguageAll.Language.VerifyOTP}!", $"{model.Username}: {LanguageAll.Language.VerifyOTP}!", 1, 200));
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LoginByEVN([FromBody] LoginEVNModel model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"LoginByEVN. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                bool IsExitst = false;
                _logger.LogInformation($"Finding EVNCode: {model.EVNCode}");
                var c = new Claim("EVNCode", model.EVNCode);
                var users = await _userManager.GetUsersForClaimAsync(c);
                if (users != null)
                {
                    if (users.Count > 0)// EVNCode Exists
                    {
                        IsExitst = true;
                        _logger.LogInformation($"Found: {model.EVNCode}");
                        var result = await _signInManager.PasswordSignInAsync(users[0].UserName, _configuration["Password:Default"], bool.Parse(_configuration["Password:RememberLogin"]), lockoutOnFailure: true);
                        if (result.Succeeded || result.RequiresTwoFactor)
                        {
                            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                            return await SendSMSOTP(users[0], new LoginModel() { Username = users[0].UserName });
                        }
                        else if (result.IsLockedOut)
                        {
                            _logger.LogWarning("User account locked out.");
                            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                            return StatusCode(StatusCodes.Status200OK,
                                        new ResponseBase(LanguageAll.Language.Fail, $"{model.EVNCode}: {LanguageAll.Language.AccountLockout}!", $"{model.EVNCode}: {LanguageAll.Language.AccountLockout}!", 0, 400));
                        }
                    }
                }

                if (!IsExitst)
                {
                    for (var i = 0; i < companyConfig.Companys.Count; i++)
                    {
                        var inv = new EVNCodeInput()
                        {
                            CompanyID = companyConfig.Companys[i].Info.CompanyId,
                            EVNCode = model.EVNCode
                        };
                        var a = await _iInvoiceServices.invoiceServices.getCustomerInfo(inv);
                        if (a.DataStatus == "00")
                        {
                            AppUser user;
                            if (a.ItemsData[0].Email.IsValidEmail())
                                user = new()
                                {
                                    SecurityStamp = Guid.NewGuid().ToString(),
                                    UserName = a.ItemsData[0].Mobile,
                                    Email = a.ItemsData[0].Email,
                                    PhoneNumber = a.ItemsData[0].Mobile,
                                    TwoFactorEnabled = true,
                                };
                            else
                                user = new()
                                {
                                    SecurityStamp = Guid.NewGuid().ToString(),
                                    UserName = a.ItemsData[0].Mobile,
                                    PhoneNumber = a.ItemsData[0].Mobile,
                                    TwoFactorEnabled = true,
                                };
                            var result = await _userManager.CreateAsync(user, _configuration["Password:Default"]);
                            if (result.Succeeded)
                            {
                                var userExists = await _userManager.FindByNameAsync(a.ItemsData[0].Mobile);
                                // Add role customer
                                await _userManager.AddToRoleAsync(userExists, "Customer");
                                // Update address
                                var a1 = await _userManager.GetClaimsAsync(userExists);
                                if (!String.IsNullOrEmpty(a.ItemsData[0].Address))
                                {
                                    await _userManager.AddClaimAsync(userExists, new Claim("Address", a.ItemsData[0].Address));
                                }
                                if (!String.IsNullOrEmpty(a.ItemsData[0].WaterIndexCode))
                                {
                                    await _userManager.AddClaimAsync(userExists, new Claim("WaterCode", a.ItemsData[0].WaterIndexCode));
                                }
                                if (!String.IsNullOrEmpty(a.ItemsData[0].TaxCode))
                                {
                                    await _userManager.AddClaimAsync(userExists, new Claim("TaxCode", a.ItemsData[0].TaxCode));
                                }
                                await _userManager.AddClaimAsync(userExists, new Claim("Fullname", a.ItemsData[0].CustomerName));
                                await _userManager.AddClaimAsync(userExists, c); // EVNCode
                                foreach (var customerCode in a.ItemsData)
                                {
                                    var _claim = new Claim("GetInvoice", $"{inv.CompanyID}.{customerCode.CustomerCode}");
                                    await _userManager.AddClaimAsync(userExists, _claim); // 
                                    await _iInvoiceServices.contractServices.AddAsync(new EntityFramework.API.Entities.Contract()
                                    {
                                        Address = customerCode.Address,
                                        CompanyId = inv.CompanyID,
                                        CustomerCode = customerCode.CustomerCode,
                                        CustomerName = customerCode.CustomerName,
                                        CustomerType = customerCode.CustomerType,
                                        Email = customerCode.Email,
                                        Mobile = customerCode.Mobile,
                                        TaxCode = customerCode.TaxCode,
                                        UserId = userExists.Id,
                                        WaterIndexCode = customerCode.WaterIndexCode
                                    });
                                }

                                // Send SMS
                                //userExists.TotalOTP = 1;
                                //userExists.OTPSendTime = DateTime.Now;
                                //await _userManager.UpdateAsync(userExists);
                                //var code = await _userManager.GenerateChangePhoneNumberTokenAsync(userExists, a.ItemsData.Mobile);
                                //var message = $"Your security code is: {code}";
                                //_smsVietel.SendSMS(await _userManager.GetPhoneNumberAsync(userExists), message);
                                //return Ok(new ResponseBase(LanguageAll.Language.VerifyOTP, $"{a.ItemsData.Mobile}: {LanguageAll.Language.VerifyOTP}!", $"{a.ItemsData.Mobile}: {LanguageAll.Language.VerifyOTP}!", 1, 200));
                                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                                return await SendSMSOTP(userExists, new LoginModel() { Username = userExists.UserName });
                            }
                        }
                    }

                }

                if (!IsExitst)
                {
                    _logger.LogInformation($"Not found: {model.EVNCode}");
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return StatusCode(StatusCodes.Status200OK, new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.NotFound,
                        MoreInfo = LanguageAll.Language.NotFound,
                        Status = 0,
                        UserMessage = LanguageAll.Language.NotFound,
                        data = null
                    });
                }
            }

            _logger.LogInformation($"Not found: {model.EVNCode}");
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.Fail, $"{model.EVNCode}: {LanguageAll.Language.NotFound}!", LanguageAll.Language.Fail));
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> PushDeviceID([FromBody] DeviceModel model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"PushDeviceID/model: {JsonConvert.SerializeObject(model)}");
            AppUserDevice device = null;
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync(HttpContext.User);
                if (user != null)
                {
                    if (!String.IsNullOrEmpty(model.DeviceId))
                    {
                        if (!String.IsNullOrEmpty(model.Token) && model.IsGetNotice)
                        {
                            if ((int)model.OS == 1 || (int)model.OS == 2)
                            {
                                Expression<Func<AppUserDevice, bool>> sqlWhere = u => (u.DeviceID == model.DeviceId);
                                device = await _iUserDeviceRepository.GetAsync(sqlWhere);
                                if (device != null)
                                {
                                    _logger.LogInformation($"PushDeviceID/model: {JsonConvert.SerializeObject(model)} --> Update");
                                    //if (device.Username == user.UserName)
                                    //{
                                        device.Token = model.Token;
                                        device.OS = model.OS;
                                        device.IsGetNotice = model.IsGetNotice;
                                        await _iUserDeviceRepository.Update(device);
                                    //await SetDeviceToClaim(model.DeviceId, model.IsGetNotice ? "1" : "0");
                                    //}
                                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                                    return Ok(new ResponseOK
                                    {
                                        Status = 1,
                                        UserMessage = $"Update device/Token {model.DeviceId}/{user.UserName} success",
                                        InternalMessage = $"Update device/Token {model.DeviceId}/{user.UserName} success",
                                        Code = 200,
                                        MoreInfo = $"Update device/Token {model.DeviceId}/{user.UserName} success",
                                        data = device
                                    });
                                }
                                else
                                {
                                    _logger.LogInformation($"PushDeviceID/model: {Newtonsoft.Json.JsonConvert.SerializeObject(model)} --> Addnew");
                                    device = new AppUserDevice()
                                    {
                                        Token = model.Token,
                                        DeviceID = model.DeviceId,
                                        IsGetNotice = model.IsGetNotice,
                                        OS = model.OS,
                                        Username = user.UserName
                                    };
                                    await _iUserDeviceRepository.AddAsync(device);
                                    //await SetDeviceToClaim(model.DeviceId, model.IsGetNotice ? "1" : "0");
                                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                                    return Ok(new ResponseOK
                                    {
                                        Status = 1,
                                        UserMessage = $"Addnew device/Token {model.DeviceId}/{user.UserName} success",
                                        InternalMessage = $"Addnew device/Token {model.DeviceId}/{user.UserName} success",
                                        Code = 200,
                                        MoreInfo = $"Addnew device/Token {model.DeviceId}/{user.UserName} success",
                                        data = device
                                    });
                                }
                            }
                        }
                    }
                }
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return Ok(new ResponseOK
            {
                Status = 0,
                UserMessage = $"{model.DeviceId}: Device/Token/Os is wrong. Update/Insert fail",
                InternalMessage = $"{model.DeviceId}: Device/Token/Os is wrong. Update/Insert fail",
                Code = 500,
                MoreInfo = $"{model.DeviceId}: Device/Token/Os is wrong. Update/Insert fail",
                data = device
            });
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> IsSetNotice([FromBody] DeviceTokenModel model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"IsSetNoticeDeviceID/model: {JsonConvert.SerializeObject(model)}");
            AppUserDevice device = null;
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync(HttpContext.User);
                if (user != null)
                {
                    Expression<Func<AppUserDevice, bool>> sqlWhere = u => (u.Token == model.Token);
                    device = await _iUserDeviceRepository.GetAsync(sqlWhere);
                    if (device != null)
                    {
                        _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                        return Ok(new ResponseOK
                        {
                            Status = 1,
                            UserMessage = $"IsSetNotice {device.DeviceID}/{user.UserName} is {(device.IsGetNotice ? "Open" : "Close")}",
                            InternalMessage = $"IsSetNotice {device.DeviceID}/{user.UserName} is {(device.IsGetNotice ? "Open" : "Close")}",
                            Code = 200,
                            MoreInfo = $"IsSetNotice {device.DeviceID}/{user.UserName} is {(device.IsGetNotice ? "Open" : "Close")}",
                            data = new DeviceModel()
                            {
                                DeviceId = device.DeviceID,
                                Token = device.Token,
                                IsGetNotice = device.IsGetNotice,
                                OS = device.OS
                            }
                        });
                    }
                    else
                    {
                        string deviceId = HttpContext.User.Claims.Where(u => u.Type == "DeviceId").FirstOrDefault()?.Value;
                        Expression<Func<AppUserDevice, bool>> sqlWhere1 = u => (u.DeviceID == deviceId);
                        device = await _iUserDeviceRepository.GetAsync(sqlWhere1);
                        _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                        return Ok(new ResponseOK
                        {
                            Status = 1,
                            UserMessage = $"IsSetNotice {device.DeviceID}/{user.UserName} is {(device.IsGetNotice ? "Open" : "Close")}",
                            InternalMessage = $"IsSetNotice {device.DeviceID}/{user.UserName} is {(device.IsGetNotice ? "Open" : "Close")}",
                            Code = 200,
                            MoreInfo = $"IsSetNotice {device.DeviceID}/{user.UserName} is {(device.IsGetNotice ? "Open" : "Close")}",
                            data = new DeviceModel()
                            {
                                DeviceId = device.DeviceID,
                                Token = model.Token,
                                IsGetNotice = false,
                                OS = device.OS
                            }
                        });
                    }
                }
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return Ok(new ResponseOK
            {
                Status = 0,
                UserMessage = $"{model.Token}: not found",
                InternalMessage = $"{model.Token}: not found",
                Code = 500,
                MoreInfo = $"{model.Token}: not found",
                data = new DeviceModel()
                {
                    DeviceId = null,
                    Token = model.Token,
                    IsGetNotice = false,
                    OS = OSType.Android
                }
            });
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetNoticeByDevice([FromBody] NoticeModel model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"GetNoticeByDevice. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            string DeviceId = HttpContext.User.Claims.Where(u => u.Type == "DeviceId").FirstOrDefault()?.Value;
            AppUserDevice device = null;
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync(HttpContext.User);
                if (user != null)
                {
                    int NoticeTypeId = 0;
                    bool IsRead = false; bool _IsReadChk = false; bool _AuthorChk = false;
                    DateTime FromDate = DateTime.Now; bool _FromDateChk = false;
                    DateTime ToDate = DateTime.Now; bool _ToDateChk = false;
                    if (model.NoticeTypeId.HasValue) NoticeTypeId = model.NoticeTypeId.Value;
                    if (model.IsRead.HasValue)
                    {
                        IsRead = model.IsRead.Value;
                        _IsReadChk = true;
                    }
                    if (model.FromDate.HasValue)
                    {
                        FromDate = model.FromDate.Value;
                        _FromDateChk = true;
                    }
                    if (model.ToDate.HasValue)
                    {
                        ToDate = model.ToDate.Value;
                        _ToDateChk = true;
                    }
                    if (!String.IsNullOrEmpty(model.Author)) _AuthorChk = true;
                    int Page = 1; int PageSize = 10;
                    if (model.Page > 0) Page = model.Page;
                    if (model.PageSize > 0) PageSize = model.PageSize;
                    Expression<Func<Notice, bool>> sqlWhere = u => (u.DeviceID == DeviceId && u.Username == user.UserName &&
                        (NoticeTypeId == 0 || u.NoticeTypeId == NoticeTypeId) &&
                        (!_IsReadChk || u.IsRead == IsRead) &&
                        (!_FromDateChk || u.CreateDate >= FromDate) &&
                        (!_ToDateChk || u.CreateDate <= ToDate) &&
                        (!_AuthorChk || u.Author.Contains(model.Author)));
                    Func<Notice, object> sqlOrder = s => s.Id;
                    var r = await _iInvoiceServices.noticeServices.GetListAsync(sqlWhere, sqlOrder, true, Page, PageSize);
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return Ok(new ResponseOK
                    {
                        Status = 1,
                        UserMessage = $"Get notice of the device {DeviceId}/{user.UserName} success",
                        InternalMessage = $"Get notice of the device {DeviceId}/{user.UserName} success",
                        Code = 200,
                        MoreInfo = $"Get notice of the device {DeviceId}/{user.UserName} success",
                        data = r
                    });
                }
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return Ok(new ResponseOK
            {
                Status = 0,
                UserMessage = $"{DeviceId}: Device is wrong. Get notice fail",
                InternalMessage = $"{DeviceId}: Device is wrong. Get notice fail",
                Code = 500,
                MoreInfo = $"{DeviceId}: Device is wrong. Get notice fail",
                data = device
            });
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SetReadNotice([FromBody] NoticeIdListModel model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                await _iInvoiceServices.noticeServices.UpdateRead(model.Ids);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return Ok(new ResponseOK
            {
                Status = 1,
                UserMessage = $"Set read notice Ok",
                InternalMessage = $"Set read notice Ok",
                Code = 200,
                MoreInfo = $"Set read notice Ok",
                data = null
            });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> PushNoticeToDevice([FromBody] NoticeInputModel model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            //int cntDeviceID = 0;
            _logger.LogInformation($"PushNoticeToDevice. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            string IP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            string token = Request.Headers["Authorization"].ToString().Substring("Bearer ".Length);
            _logger.LogInformation($"IP: {IP}; token: {token}: {((fireBaseAPIConfig.IPTrust.Contains(IP) || fireBaseAPIConfig.IPTrust.Contains("*")) && token == fireBaseAPIConfig.ServerKey)}");
            if ((fireBaseAPIConfig.IPTrust.Contains(IP) || fireBaseAPIConfig.IPTrust.Contains("*")) && token == fireBaseAPIConfig.ServerKey)
            {
                // finding user/deviceid
                IList<AppUser> _u = new List<AppUser>();
                if (model.CustomerCode == "*")
                {
                    _u = await _userManager.GetUsersInRoleAsync("Customer");
                }                    
                else
                {
                    var _claim = new Claim("GetInvoice", $"{model.CompanyId}.{model.CustomerCode}");
                    _u = await _userManager.GetUsersForClaimAsync(_claim);
                }
                
                _logger.LogInformation($"GetInvoice: {model.CompanyId}.{model.CustomerCode}; Count: {_u.Count}");
                model.Content = model.Content.XSSFilter(htmlXSS);
                if (_u.Count > 0)
                {
                    List<Notice> notices = new List<Notice>();
                    foreach (var u in _u)
                    {
                        List<string> device_ids = new List<string>();
                        List<string> token_ids = new List<string>();
                        Expression<Func<AppUserDevice, bool>> sqlWhere = d => (d.Username == u.UserName);
                        var d = await _iUserDeviceRepository.GetManyAsync(sqlWhere);
                        foreach (var d1 in d)
                        {
                            notices.Add(new Notice()
                            {
                                Author = model.Author,
                                Content = model.Content,
                                CreateDate = DateTime.Now,
                                DeviceID = d1.DeviceID,
                                IsDelete = false,
                                IsHTML = model.IsHTML,
                                IsRead = false,
                                NoticeTypeId = model.NoticeTypeId,
                                NoticeTypeName = model.NoticeTypeName,
                                OS = d1.OS,
                                Subject = model.Subject,
                                Username = d1.Username,
                                Link = model.Link
                            });
                            if (d1.IsGetNotice && !String.IsNullOrEmpty(d1.Token) && !String.IsNullOrEmpty(d1.DeviceID))
                            {
                                //cntDeviceID = cntDeviceID + 1;
                                token_ids.Add(d1.Token);
                                device_ids.Add(d1.DeviceID);
                                //if (cntDeviceID % 1000 == 0)
                                //{
                                //    await PushFirebase(token_ids, device_ids, model, u);
                                //    device_ids = new List<string>();
                                //    token_ids = new List<string>();
                                //    await Task.Delay(500);
                                //}
                            }
                        }
                        await PushFirebase(token_ids, device_ids, model, u);
                    }
                    if (notices.Count > 0)
                    {
                        await _iInvoiceServices.noticeServices.AddManyAsync(notices);
                        return Ok(new ResponseOK
                        {
                            Status = 1,
                            UserMessage = $"{model.CustomerCode}: Push notice success",
                            InternalMessage = $"{model.CustomerCode}: Push notice success",
                            Code = 200,
                            MoreInfo = $"{model.CustomerCode}: Push notice success",
                            data = notices
                        });
                    }
                }
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return Ok(new ResponseOK
            {
                Status = 0,
                UserMessage = $"{model.CustomerCode}: Push notice fail",
                InternalMessage = $"{model.CustomerCode}: Push notice fail",
                Code = 500,
                MoreInfo = $"{model.CustomerCode}: Push notice fail",
                data = null
            });
        }

        private async Task PushFirebase(List<string> token_ids, List<string> device_ids, NoticeInputModel model, AppUser u)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            if (token_ids.Count > 0)
            {
                var model1 = new NoticePushFirebaseModel()
                {
                    Author = model.Author,
                    Content = model.Content,
                    DeviceID = device_ids,
                    Token = token_ids,
                    IsHTML = model.IsHTML,
                    IsRead = false,
                    NoticeTypeId = model.NoticeTypeId,
                    NoticeTypeName = model.NoticeTypeName,
                    Subject = model.Subject,
                    Link = model.Link,
                    Username = u.UserName
                };
                await Tools.PushFireBase(model1, fireBaseConfig, _logger);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> SyncContract()
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");

            var userExists = await _userManager.GetUserAsync(HttpContext.User);

            for (var i = 0; i < companyConfig.Companys.Count; i++)
            {
                var inv = new EVNCodeInput()
                {
                    CompanyID = companyConfig.Companys[i].Info.CompanyId,
                    EVNCode = userExists.UserName
                };
                var a = await _iInvoiceServices.invoiceServices.getCustomerInfo(inv);
                if (a.DataStatus == "00")
                {
                    // Update address                    
                    var a1 = await _userManager.GetClaimsAsync(userExists);
                    Claim claim;
                    if (!String.IsNullOrEmpty(a.ItemsData[0].Address))
                    {
                        claim = a1.Where(u => u.Type == "Address").FirstOrDefault();
                        if (claim != null) await _userManager.RemoveClaimAsync(userExists, claim);
                        await _userManager.AddClaimAsync(userExists, new Claim("Address", a.ItemsData[0].Address));
                    }
                    if (!String.IsNullOrEmpty(a.ItemsData[0].WaterIndexCode))
                    {
                        claim = a1.Where(u => u.Type == "WaterCode").FirstOrDefault();
                        if (claim != null) await _userManager.RemoveClaimAsync(userExists, claim);
                        await _userManager.AddClaimAsync(userExists, new Claim("WaterCode", a.ItemsData[0].WaterIndexCode));
                    }
                    if (!String.IsNullOrEmpty(a.ItemsData[0].TaxCode))
                    {
                        claim = a1.Where(u => u.Type == "TaxCode").FirstOrDefault();
                        if (claim != null) await _userManager.RemoveClaimAsync(userExists, claim);
                        await _userManager.AddClaimAsync(userExists, new Claim("TaxCode", a.ItemsData[0].TaxCode));
                    }
                    claim = a1.Where(u => u.Type == "Fullname").FirstOrDefault();
                    if (claim != null) await _userManager.RemoveClaimAsync(userExists, claim);
                    await _userManager.AddClaimAsync(userExists, new Claim("Fullname", a.ItemsData[0].CustomerName));

                    foreach (var customerCode in a.ItemsData)
                    {
                        Expression<Func<EntityFramework.API.Entities.Contract, bool>> expression = u => (
                            (u.CompanyId >= inv.CompanyID) &&
                            (u.CustomerCode == customerCode.CustomerCode));
                        var contract = await _iInvoiceServices.contractServices.GetAsync(expression);
                        if (contract == null)
                        {
                            var _claim = new Claim("GetInvoice", $"{inv.CompanyID}.{customerCode.CustomerCode}");
                            await _userManager.AddClaimAsync(userExists, _claim); //
                            await _iInvoiceServices.contractServices.AddAsync(new EntityFramework.API.Entities.Contract()
                            {
                                Address = customerCode.Address,
                                CompanyId = inv.CompanyID,
                                CustomerCode = customerCode.CustomerCode,
                                CustomerName = customerCode.CustomerName,
                                CustomerType = customerCode.CustomerType,
                                Email = customerCode.Email,
                                Mobile = customerCode.Mobile,
                                TaxCode = customerCode.TaxCode,
                                UserId = userExists.Id,
                                WaterIndexCode = customerCode.WaterIndexCode
                            });
                        }
                    }
                }
            } 
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.Success, $"{userExists.UserName}: {LanguageAll.Language.Success}!", LanguageAll.Language.Success, 0, 200));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SyncContract([FromBody] LoginModel model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            //int cntDeviceID = 0;
            _logger.LogInformation($"PushNoticeToDevice. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            string IP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            string token = Request.Headers["Authorization"].ToString().Substring("Bearer ".Length);
            _logger.LogInformation($"IP: {IP}; token: {token}: {((fireBaseAPIConfig.IPTrust.Contains(IP) || fireBaseAPIConfig.IPTrust.Contains("*")) && token == fireBaseAPIConfig.ServerKey)}");
            if ((fireBaseAPIConfig.IPTrust.Contains(IP) || fireBaseAPIConfig.IPTrust.Contains("*")) && token == fireBaseAPIConfig.ServerKey)
            {
                var userExists = await _userManager.FindByNameAsync(model.Username);
                if (userExists != null)
                {
                    for (var i = 0; i < companyConfig.Companys.Count; i++)
                    {
                        var inv = new EVNCodeInput()
                        {
                            CompanyID = companyConfig.Companys[i].Info.CompanyId,
                            EVNCode = userExists.UserName
                        };
                        var a = await _iInvoiceServices.invoiceServices.getCustomerInfo(inv);
                        if (a.DataStatus == "00")
                        {
                            // Update address                    
                            var a1 = await _userManager.GetClaimsAsync(userExists);
                            Claim claim;
                            if (!String.IsNullOrEmpty(a.ItemsData[0].Address))
                            {
                                claim = a1.Where(u => u.Type == "Address").FirstOrDefault();
                                if (claim != null) await _userManager.RemoveClaimAsync(userExists, claim);
                                await _userManager.AddClaimAsync(userExists, new Claim("Address", a.ItemsData[0].Address));
                            }
                            if (!String.IsNullOrEmpty(a.ItemsData[0].WaterIndexCode))
                            {
                                claim = a1.Where(u => u.Type == "WaterCode").FirstOrDefault();
                                if (claim != null) await _userManager.RemoveClaimAsync(userExists, claim);
                                await _userManager.AddClaimAsync(userExists, new Claim("WaterCode", a.ItemsData[0].WaterIndexCode));
                            }
                            if (!String.IsNullOrEmpty(a.ItemsData[0].TaxCode))
                            {
                                claim = a1.Where(u => u.Type == "TaxCode").FirstOrDefault();
                                if (claim != null) await _userManager.RemoveClaimAsync(userExists, claim);
                                await _userManager.AddClaimAsync(userExists, new Claim("TaxCode", a.ItemsData[0].TaxCode));
                            }
                            claim = a1.Where(u => u.Type == "Fullname").FirstOrDefault();
                            if (claim != null) await _userManager.RemoveClaimAsync(userExists, claim);
                            await _userManager.AddClaimAsync(userExists, new Claim("Fullname", a.ItemsData[0].CustomerName));

                            foreach (var customerCode in a.ItemsData)
                            {
                                Expression<Func<EntityFramework.API.Entities.Contract, bool>> expression = u => (
                                    (u.CompanyId >= inv.CompanyID) &&
                                    (u.CustomerCode == customerCode.CustomerCode));
                                var contract = await _iInvoiceServices.contractServices.GetAsync(expression);
                                if (contract == null)
                                {
                                    var _claim = new Claim("GetInvoice", $"{inv.CompanyID}.{customerCode.CustomerCode}");
                                    await _userManager.AddClaimAsync(userExists, _claim); //
                                    await _iInvoiceServices.contractServices.AddAsync(new EntityFramework.API.Entities.Contract()
                                    {
                                        Address = customerCode.Address,
                                        CompanyId = inv.CompanyID,
                                        CustomerCode = customerCode.CustomerCode,
                                        CustomerName = customerCode.CustomerName,
                                        CustomerType = customerCode.CustomerType,
                                        Email = customerCode.Email,
                                        Mobile = customerCode.Mobile,
                                        TaxCode = customerCode.TaxCode,
                                        UserId = userExists.Id,
                                        WaterIndexCode = customerCode.WaterIndexCode
                                    });
                                }
                            }
                        }
                    }
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return StatusCode(StatusCodes.Status200OK,
                        new ResponseBase(LanguageAll.Language.Success, $"{userExists.UserName}: {LanguageAll.Language.Success}!", LanguageAll.Language.Success, 0, 200));
                }
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return Ok(new ResponseOK
            {
                Status = 0,
                UserMessage = $"{model.Username}: Push notice fail",
                InternalMessage = $"{model.Username}: Push notice fail",
                Code = 500,
                MoreInfo = $"{model.Username}: Push notice fail",
                data = null
            });
        }
        //private async Task SetDeviceToClaim(string DeviceId, string IsGetNotice = "0")
        //{
        //    var u = await _userManager.GetUserAsync(User);
        //    var a = await _userManager.GetClaimsAsync(u);
        //    var _d = a.Where(u => u.Type == "DeviceId").FirstOrDefault();
        //    if (_d == null)
        //    {
        //        await _userManager.AddClaimAsync(u, new Claim("DeviceId", DeviceId));
        //    }
        //    else
        //    {
        //        await _userManager.ReplaceClaimAsync(u, _d, new Claim("DeviceId", DeviceId));
        //    }

        //    var _n = a.Where(u => u.Type == "IsGetNotice").FirstOrDefault();
        //    if (_n == null)
        //    {
        //        await _userManager.AddClaimAsync(u, new Claim("IsGetNotice", IsGetNotice));
        //    }
        //    else
        //    {
        //        await _userManager.ReplaceClaimAsync(u, _n, new Claim("IsGetNotice", IsGetNotice));
        //    }
        //}
    }
}
