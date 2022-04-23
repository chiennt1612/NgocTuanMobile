using Auth.Models;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SMSGetway;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils;
using Utils.Models;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ISMSVietel _smsVietel;
        private readonly IJWTToken _jwtToken;
        private readonly ILogger<AuthenticateController> _logger;

        public AuthenticateController(
            ILogger<AuthenticateController> logger,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IConfiguration configuration,
            ISMSVietel smsVietel,
            IJWTToken jwtToken)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _smsVietel = smsVietel;
            _logger = logger;
            _jwtToken = jwtToken;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _jwtToken.GenerateToken(authClaims);
                var refreshToken = _jwtToken.GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

                await _userManager.UpdateAsync(user);

                return Ok(new LoginSuccessModel
                {
                    Status = 0,
                    UserMessage = "Login fail!",
                    InternalMessage = "Login fail!",
                    Code = 500,
                    data = new LoginData
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        RefreshToken = refreshToken,
                        Expiration = token.ValidTo
                    }                    
                });
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase { Status = 0, UserMessage = "Login fail!", InternalMessage = "Login fail!", Code = 500 });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] LoginModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase { Status = 0, UserMessage = "User already exists!", InternalMessage = "User already exists!", Code=500 });

            AppUser user = new()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase { Status = 0, UserMessage = "User creation failed! Please check user details and try again!", InternalMessage = "User creation failed! Please check user details and try again!", Code = 500 });

            return Ok(new ResponseBase { Status = 1, UserMessage = "User created successfully!", InternalMessage = "User created successfully!", Code = 200 });
        }

    }
}
