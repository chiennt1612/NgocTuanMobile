using Auth.Helper;
using Auth.Models;
using EntityFramework.API.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
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
    [SecurityHeaders]
    public class AuthenticateController : ControllerBase
    {
        private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ISMSVietel _smsVietel;
        private readonly ITokenCreationService _jwtToken;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly IStringLocalizer<AuthenticateController> _localizer;

        public AuthenticateController(
            ILogger<AuthenticateController> logger,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            IConfiguration configuration,
            ISMSVietel smsVietel,
            ITokenCreationService jwtToken,
            IStringLocalizer<AuthenticateController> localizer,
            IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _smsVietel = smsVietel;
            _logger = logger;
            _jwtToken = jwtToken;
            _signInManager = signInManager;
            _localizer = localizer;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
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
                        //var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
                        //var message = $"Your security code is: {code}";
                        //_smsVietel.SendSMS(await _userManager.GetPhoneNumberAsync(user), message);
                        var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.Username);
                        var message = $"Your security code is: {code}";
                        _smsVietel.SendSMS(await _userManager.GetPhoneNumberAsync(user), message);
                        return Ok(new ResponseBase("Please input OTP!", $"{model.Username}: Please input OTP!", $"OTP!", 1, 200));
                    }
                    else if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            new ResponseBase("Login fail!", $"{model.Username}: User account locked out!", "Login fail!"));
                    }
                }
            }

            _logger.LogInformation($"Not found: {model.Username}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseBase("Login fail!", $"{model.Username}: Login fail!", "Login fail!"));
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
                        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase("User already exists!", "User already exists!", "User already exists!"));
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
                        var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.Username);
                        var message = $"Your security code is: {code}";
                        _smsVietel.SendSMS(await _userManager.GetPhoneNumberAsync(user), message);

                        return Ok(new ResponseBase("Please input OTP!", $"{model.Username}: Please input OTP!", $"OTP!", 1, 200));
                    }
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseBase("User creation failed! Please check user details and try again!", "User creation failed! Please check user details and try again!", "User creation failed! Please check user details and try again!"));
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
                new ResponseBase("Invalid code.", $"Invalid code.", "Login fail!"));
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
                new ResponseBase("Renew Token fail!", "Renew Token fail!", "Renew Token fail!"));
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Revoke()
        {
            var user = await GetCurrentUserAsync();

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return Ok(new ResponseBase("Revoke OK", $"Revoke OK", "Revoke OK", 1, 200));
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
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }
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
    }
}
