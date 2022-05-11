using Auth.Helper;
using Auth.Models;
using Auth.Services;
using EntityFramework.API.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SMSGetway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public AuthenticateController(
            ILogger<AuthenticateController> logger,
            AppUserManager userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration,
            ISMSVietel smsVietel,
            ITokenCreationService jwtToken,
            IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
            LoginConfiguration loginConfiguration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _smsVietel = smsVietel;
            _logger = logger;
            _jwtToken = jwtToken;
            _signInManager = signInManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _loginConfiguration = loginConfiguration;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            _logger.LogInformation($"ModelState: {ModelState.IsValid}");
            if (ModelState.IsValid)
            {
                _logger.LogInformation($"Finding username: {model.Username}");
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    _logger.LogInformation($"Found: {model.Username}");
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, _configuration["Password:Default"], bool.Parse(_configuration["Password:RememberLogin"]), lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        return await LoginOK(user);
                    }
                    else if (result.RequiresTwoFactor)
                    {
                        return await SendSMSOTP(user, model);
                    }
                    else if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return StatusCode(StatusCodes.Status500InternalServerError,
                                    new ResponseBase(LanguageAll.Language.Fail, $"{model.Username}: {LanguageAll.Language.AccountLockout}!", $"{model.Username}: {LanguageAll.Language.AccountLockout}!"));
                    }
                }
            }

            _logger.LogInformation($"Not found: {model.Username}");
            return StatusCode(StatusCodes.Status500InternalServerError,
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
        //            return StatusCode(StatusCodes.Status500InternalServerError,
        //                new ResponseBase("Login fail!", $"User account locked out!", "Login fail!"));
        //        }
        //    }
        //    _logger.LogWarning("Invalid code.");
        //    return StatusCode(StatusCodes.Status500InternalServerError,
        //        new ResponseBase("Invalid code.", $"Invalid code.", "Login fail!"));
        //}

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register([FromBody] LoginModel model)
        {
            _logger.LogInformation($"ModelState: {ModelState.IsValid}");
            if (ModelState.IsValid)
            {
                if (_smsVietel.FormatMobile(model.Username) != "")
                {
                    var userExists = await _userManager.FindByNameAsync(model.Username);
                    if (userExists != null)
                    //return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase("User already exists!", "User already exists!", "User already exists!"));
                    {
                        return await SendSMSOTP(userExists, model);
                    }
                    else
                    {
                        AppUser user = new()
                        {
                            SecurityStamp = Guid.NewGuid().ToString(),
                            UserName = model.Username,
                            PhoneNumber = model.Username,
                            TwoFactorEnabled = true,
                        };
                        var result = await _userManager.CreateAsync(user, _configuration["Password:Default"]);
                        if (result.Succeeded)
                        {
                            userExists.TotalOTP = 1;
                            userExists.OTPSendTime = DateTime.Now;
                            await _userManager.UpdateAsync(userExists);
                            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(userExists, model.Username);
                            var message = $"Your security code is: {code}";
                            _smsVietel.SendSMS(await _userManager.GetPhoneNumberAsync(userExists), message);
                            return Ok(new ResponseBase(LanguageAll.Language.VerifyOTP, $"{model.Username}: {LanguageAll.Language.VerifyOTP}!", $"{model.Username}: {LanguageAll.Language.VerifyOTP}!", 1, 200));
                        }
                    }
                }
            }
            _logger.LogError($"{model.Username}: {LanguageAll.Language.UserCreateFail}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseBase(LanguageAll.Language.UserCreateFail, LanguageAll.Language.UserCreateFail, LanguageAll.Language.UserCreateFail));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> VerifyPhoneNumber([FromBody] OTPModel model)
        {
            _logger.LogInformation($"ModelState: {ModelState.IsValid}");
            if (ModelState.IsValid)
            {
                //var user = await GetCurrentUserAsync();
                var user = await _userManager.FindByNameAsync(model.Username);
                string PhoneNumber = user.UserName;
                _logger.LogInformation($"Validating OTP: {model.Code}");
                var result = await _userManager.ChangePhoneNumberAsync(user, PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    return await LoginOK(user);
                }
            }
            _logger.LogWarning("Invalid code.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseBase(LanguageAll.Language.OTPInvalid, LanguageAll.Language.OTPInvalid, LanguageAll.Language.OTPInvalid));
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RenewToken([FromBody] RefreshTokenModel model)
        {
            _logger.LogInformation($"ModelState: {ModelState.IsValid}");
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                if (!String.IsNullOrEmpty(user.RefreshToken))
                {
                    if (user.RefreshToken == model.RefreshToken && user.RefreshTokenExpiryTime >= DateTime.Now)
                    {
                        return await LoginOK(user);
                    }
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseBase(LanguageAll.Language.RenewTokenFail, LanguageAll.Language.RenewTokenFail, LanguageAll.Language.RenewTokenFail));
        }

        [Authorize]
        [MyAuthorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Revoke()
        {
            var user = await GetCurrentUserAsync();

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

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

        private async Task<IActionResult> LoginOK(AppUser user)
        {
            _logger.LogInformation($"Logined: {user.UserName}");
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

        private Task<AppUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private async Task<IActionResult> SendSMSOTP(AppUser user, LoginModel model)
        {
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
            switch (IsNotAllowSend)
            {
                case 2:
                    _logger.LogWarning("You needs request OTP after 3 minutes.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new ResponseBase(LanguageAll.Language.Fail, $"{model.Username}: {LanguageAll.Language.OTPWait}!", $"{model.Username}: {LanguageAll.Language.OTPWait}!"));
                case 3:
                    _logger.LogWarning("You request too more OTP on this day.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new ResponseBase(LanguageAll.Language.Fail, $"{model.Username}: {LanguageAll.Language.OTPLimited}!", $"{model.Username}: {LanguageAll.Language.OTPLimited}!"));
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
                    return Ok(new ResponseBase(LanguageAll.Language.VerifyOTP, $"{model.Username}: {LanguageAll.Language.VerifyOTP}!", $"{model.Username}: {LanguageAll.Language.VerifyOTP}!", 1, 200));
            }
        }
    }
}
