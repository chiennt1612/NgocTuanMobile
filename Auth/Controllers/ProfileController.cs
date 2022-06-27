using Auth.Models;
using Auth.Services;
using Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
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
    //[ValidateModel]
    //[ValidateAntiForgeryToken]
    public class ProfileController : ControllerBase
    {
        private readonly IProfile _profile;
        private readonly AppUserManager _userManager;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IProfile profile, AppUserManager userManager, ILogger<ProfileController> logger)
        {
            _profile = profile;
            _userManager = userManager;
            _logger = logger;
            _logger.WriteLog("Starting profile");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetCompanyList()
        {
            var a = await _profile.GetCompanyList();
            _logger.WriteLog($"GetCompanyList: {a.UserMessage}", "GetCompanyList");
            return Ok(a);
        }

        [HttpGet]
        [Route("[action]/{CompanyID}")]
        public async Task<IActionResult> GetContractList(int CompanyID)
        {
            ContractInput inv = new ContractInput()
            {
                CompanyID = CompanyID,
                Mobile = (await _userManager.GetUserAsync(HttpContext.User)).UserName
            };
            var a = await _profile.GetContractAllList(inv);
            _logger.WriteLog($"GetContractList {inv.CompanyID}/ {inv.Mobile}: {a.UserMessage}", $"GetContractList {inv.CompanyID}/ {inv.Mobile}");
            return Ok(a);
        }

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<IActionResult> SetCompanyInfo([FromBody] CompanyInfoInput model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var a = await _profile.SetCompanyInfo(model);
        //        if (a.Status == 0)
        //        {
        //            switch (a.Code)
        //            {
        //                case 400:
        //                    return StatusCode(StatusCodes.Status200OK, a);
        //                default:
        //                    return StatusCode(StatusCodes.Status200OK, a);

        //            }
        //        }
        //        return Ok(a);
        //    }
        //    else
        //    {
        //        return StatusCode(StatusCodes.Status200OK, new ResponseOK()
        //        {
        //            Code = 400,
        //            InternalMessage = LanguageAll.Language.SetProfileFailEmail,
        //            MoreInfo = LanguageAll.Language.SetProfileFailEmail,
        //            Status = 0,
        //            UserMessage = LanguageAll.Language.SetProfileFailEmail,
        //            data = ModelState.ToList()
        //        });
        //    }
        //}

        //[HttpPost]
        //[Route("[action]/{IsToken}")]
        //public async Task<IActionResult> GetCompanyInfo(int IsToken)
        //{
        //    var a = await _profile.GetCompanyInfo(IsToken);
        //    return Ok(a);
        //}

        [HttpPost]
        [Route("[action]/{IsToken}")]
        public async Task<IActionResult> GetProfile(int IsToken)
        {
            var a = await _profile.GetProfile(IsToken);
            _logger.WriteLog($"GetProfile {IsToken}: {a.UserMessage}", $"GetProfile {IsToken}");
            return Ok(a);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SetProfile([FromBody] ProfileInputModel model)
        {
            if (ModelState.IsValid)
            {
                var a = await _profile.SetProfile(model);
                _logger.WriteLog($"SetProfile: {a.UserMessage}", $"SetProfile");
                if (a.Status == 0)
                {
                    switch (a.Code)
                    {
                        case 400:
                            return StatusCode(StatusCodes.Status200OK, a);
                        default:
                            return StatusCode(StatusCodes.Status200OK, a);

                    }
                }
                return Ok(a);
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.SetProfileFailEmail,
                    MoreInfo = LanguageAll.Language.SetProfileFailEmail,
                    Status = 0,
                    UserMessage = LanguageAll.Language.SetProfileFailEmail,
                    data = ModelState.ToList()
                });
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LinkContract([FromBody] InvoiceInput inv)
        {
            var a = await _profile.LinkInvoice(inv);
            _logger.WriteLog($"LinkContract {inv.CompanyID}/ {inv.CustomerCode}: {a.UserMessage}", $"LinkContract {inv.CompanyID}/ {inv.CustomerCode}");
            if (a.Status == 0)
            {
                switch (a.Code)
                {
                    case 400:
                        return StatusCode(StatusCodes.Status200OK, a);
                    default:
                        return StatusCode(StatusCodes.Status200OK, a);

                }
            }
            return Ok(a);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RemoveContract([FromBody] InvoiceInput inv)
        {
            var a = await _profile.RemoveInvoice(inv);
            _logger.WriteLog($"LinkContract {inv.CompanyID}/ {inv.CustomerCode}: {a.UserMessage}", $"LinkContract {inv.CompanyID}/ {inv.CustomerCode}");
            if (a.Status == 0)
            {
                switch (a.Code)
                {
                    case 400:
                        return StatusCode(StatusCodes.Status200OK, a);
                    default:
                        return StatusCode(StatusCodes.Status200OK, a);

                }
            }
            return Ok(a);
        }

    }
}
