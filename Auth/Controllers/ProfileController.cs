using Auth.Models;
using Auth.Services;
using Auth.Services.Interfaces;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    //[ValidateModel]
    //[ValidateAntiForgeryToken]
    public class ProfileController : ControllerBase
    {
        private readonly IProfile _profile;
        private readonly IContractServices _contract;
        private readonly AppUserManager _userManager;
        private readonly ILogger<ProfileController> _logger;
        public CompanyConfig companyConfig { get; set; }
        private IConfiguration _configuration;

        public ProfileController(IProfile profile, AppUserManager userManager, ILogger<ProfileController> logger,
            IConfiguration _configuration, IContractServices _contract)
        {
            _profile = profile;
            _userManager = userManager;
            _logger = logger;
            this._configuration = _configuration;
            companyConfig = this._configuration.GetSection(nameof(CompanyConfig)).Get<CompanyConfig>();
            _logger.WriteLog("Starting profile");
            this._contract = _contract;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetCompanyList()
        {
            var a = await _profile.GetCompanyList();
            _logger.WriteLog($"GetCompanyList: {a.UserMessage}", "GetCompanyList");
            return Ok(a);
        }

        #region contract from AYs
        [HttpGet]
        [Route("[action]/{CompanyID}")]
        public async Task<IActionResult> AYsContractList(int CompanyID)
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

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> AYsContractList()
        {
            var Mobile = (await _userManager.GetUserAsync(HttpContext.User)).UserName;
            List<ContractInfo> r = new List<ContractInfo>();
            foreach (var companyInfo in companyConfig.Companys)
            {
                ContractInput inv = new ContractInput()
                {
                    CompanyID = companyInfo.Info.CompanyId,
                    Mobile = Mobile
                };
                var a = await _profile.GetContractList(inv);
                if (a.DataStatus == "00")
                {
                    r.Add(new ContractInfo()
                    {
                        CompanyInfo = companyInfo.Info,
                        ContractList = a.ItemsData.ContractList
                    });
                }
            }

            if(r.Count < 1)
            {
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
            }
            _logger.WriteLog($"GetContractList");
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 0,
                UserMessage = LanguageAll.Language.Success,
                data = r
            });
        }

        [HttpGet]
        [Route("[action]/{CustomerCode}")]
        public async Task<IActionResult> AYsContractInfo(string CustomerCode)
        {
            ContractOneInfo r = new ContractOneInfo();
            foreach (var companyInfo in companyConfig.Companys)
            {
                ContractInput inv = new ContractInput()
                {
                    CompanyID = companyInfo.Info.CompanyId,
                    Mobile = CustomerCode
                };
                r.CompanyInfo = companyInfo.Info;
                var a = await _profile.GetContractList(inv);
                if (a.DataStatus == "00")
                {
                    var f = a.ItemsData.ContractList.Where(u => u.CustomerCode == CustomerCode).FirstOrDefault();
                    if (f != default)
                    {
                        r.ContractInfo = f;
                        return Ok(new ResponseOK()
                        {
                            Code = 200,
                            InternalMessage = LanguageAll.Language.Success,
                            MoreInfo = LanguageAll.Language.Success,
                            Status = 0,
                            UserMessage = LanguageAll.Language.Success,
                            data = r
                        });
                    }
                }
            }


            _logger.WriteLog($"GetContractList");
            return Ok(new ResponseOK()
            {
                Code = 404,
                InternalMessage = LanguageAll.Language.Fail,
                MoreInfo = LanguageAll.Language.Fail,
                Status = 1,
                UserMessage = LanguageAll.Language.Fail,
                data = null
            });
        }
        #endregion

        #region contract
        [HttpGet]
        [Route("[action]/{CompanyID}")]
        public async Task<IActionResult> GetContractList(int CompanyID)
        {
            var UserId = (await _userManager.GetUserAsync(HttpContext.User)).Id;
            Expression<Func<Contract, bool>> expression = u => (
                    (u.CompanyId >= CompanyID) &&
                    (u.UserId == UserId));
            var contract = await _contract.GetManyAsync(expression);
            if(contract.Count() < 1)
            {
                return Ok(new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.NotFound,
                    MoreInfo = LanguageAll.Language.NotFound,
                    Status = 0,
                    UserMessage = LanguageAll.Language.NotFound,
                    data = null
                });
            }
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 0,
                UserMessage = LanguageAll.Language.Success,
                data = new
                {
                    companyInfo = companyConfig.Companys[CompanyID].Info,
                    contractList = contract
                }
            }); 
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetContractList()
        {
            var UserId = (await _userManager.GetUserAsync(HttpContext.User)).Id;
            Expression<Func<Contract, bool>> expression = u => (u.UserId == UserId);
            var contract = await _contract.GetManyAsync(expression);
            if (contract.Count() < 1)
            {
                return Ok(new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.NotFound,
                    MoreInfo = LanguageAll.Language.NotFound,
                    Status = 0,
                    UserMessage = LanguageAll.Language.NotFound,
                    data = null
                });
            }
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 0,
                UserMessage = LanguageAll.Language.Success,
                data = new
                {
                    companyInfo = companyConfig.Companys[0].Info,
                    contractList = contract
                }
            });
        }

        [HttpGet]
        [Route("[action]/{CustomerCode}")]
        public async Task<IActionResult> GetContractInfo(string CustomerCode)
        {
            var UserId = (await _userManager.GetUserAsync(HttpContext.User)).Id;
            Expression<Func<Contract, bool>> expression = u => (u.UserId == UserId && u.CustomerCode == CustomerCode);
            var contract = await _contract.GetManyAsync(expression);
            if (contract.Count() < 1)
            {
                return Ok(new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.NotFound,
                    MoreInfo = LanguageAll.Language.NotFound,
                    Status = 0,
                    UserMessage = LanguageAll.Language.NotFound,
                    data = null
                });
            }
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 0,
                UserMessage = LanguageAll.Language.Success,
                data = new
                {
                    companyInfo = companyConfig.Companys[0].Info,
                    contractInfo = contract.ToList()[0]
                }
            });
        }
        #endregion

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
                // Check Avatar and save to admin.nuocngoctuan.com                
                if (!string.IsNullOrEmpty(model.Avatar))
                {
                    _logger.WriteLog($"1. SetProfile {model.Avatar}");
                    var username = User.Claims.Where(u => u.Type == ClaimTypes.Name).FirstOrDefault()?.Value;
                    if (string.IsNullOrEmpty(username)) username = "admin";
                    string path = companyConfig.AvatarFolder;
                    string fileName = username + ".jpg";
                    _logger.WriteLog($"2. SetProfile {path}/{fileName}");
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    if (System.IO.File.Exists(path + @"/" + fileName)) System.IO.File.Delete(path + @"/" + fileName);
                    System.IO.File.WriteAllBytes(path + @"/" + fileName, Convert.FromBase64String(model.Avatar));
                    model.Avatar = @"https://admin.nuocngoctuan.com/Upload/Avatar/" + fileName;
                    _logger.WriteLog($"3. SetProfile {model.Avatar}");
                }
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
