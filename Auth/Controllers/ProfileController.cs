using Auth.Models;
using Auth.Services;
using Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
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

        public ProfileController(IProfile profile, AppUserManager userManager)
        {
            _profile = profile;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetCompanyList()
        {
            var a = await _profile.GetCompanyList();
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
        //                    return StatusCode(StatusCodes.Status400BadRequest, a);
        //                default:
        //                    return StatusCode(StatusCodes.Status500InternalServerError, a);

        //            }
        //        }
        //        return Ok(a);
        //    }
        //    else
        //    {
        //        return StatusCode(StatusCodes.Status400BadRequest, new ResponseOK()
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
            return Ok(a);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SetProfile([FromBody] ProfileInputModel model)
        {
            if (ModelState.IsValid)
            {
                var a = await _profile.SetProfile(model);
                if (a.Status == 0)
                {
                    switch (a.Code)
                    {
                        case 400:
                            return StatusCode(StatusCodes.Status400BadRequest, a);
                        default:
                            return StatusCode(StatusCodes.Status500InternalServerError, a);

                    }
                }
                return Ok(a);
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseOK()
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
            if (a.Status == 0)
            {
                switch (a.Code)
                {
                    case 400:
                        return StatusCode(StatusCodes.Status400BadRequest, a);
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, a);

                }
            }
            return Ok(a);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RemoveContract([FromBody] InvoiceInput inv)
        {
            var a = await _profile.RemoveInvoice(inv);
            if (a.Status == 0)
            {
                switch (a.Code)
                {
                    case 400:
                        return StatusCode(StatusCodes.Status400BadRequest, a);
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, a);

                }
            }
            return Ok(a);
        }

    }
}
