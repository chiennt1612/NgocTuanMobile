using Auth.Models;
using Auth.Services.Interfaces;
using EntityFramework.API.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils.Models;
using Utils.Repository.Interfaces;

namespace Auth.Services
{
    public class Profile : IProfile
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<Profile> _logger;
        private readonly IHttpContextAccessor _context;
        private readonly IInvoiceRepository _invoice;

        public Profile(
            ILogger<Profile> logger,
            UserManager<AppUser> userManager,
            IInvoiceRepository invoice,
            IHttpContextAccessor context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _invoice = invoice;
        }

        public async Task<ResponseOK> GetProfile(int profileType = 1)
        {
            var a1 = await _userManager.GetUserAsync(_context.HttpContext.User);
            _logger.LogInformation($"Get profile: {a1.UserName}");
            IEnumerable<Claim> claims;
            if (profileType == 1)
            {
                claims = _context.HttpContext.User.Claims;
            }
            else
            {
                claims = await _userManager.GetClaimsAsync(a1);
            }

            ProfileOutputModel a = new ProfileOutputModel();
            a.Address = claims.Where(u => u.Type == "Address").FirstOrDefault()?.Value;
            if (DateTime.TryParse(claims.Where(u => u.Type == "Birthday").FirstOrDefault()?.Value, out DateTime _a))
            {
                a.Birthday = _a;
            }
            else
            {
                a.Birthday = default;
            }
            a.CompanyName = claims.Where(u => u.Type == "CompanyName").FirstOrDefault()?.Value;
            a.WaterCompany = claims.Where(u => u.Type == "WaterCompany").FirstOrDefault()?.Value;
            foreach (var item in claims.Where(u => u.Type == "GetInvoice"))
            {
                a.CustomerCodeList.Add(item.Value);
            }
            a.Email = claims.Where(u => u.Type == ClaimTypes.Email).FirstOrDefault()?.Value;
            a.Fullname = claims.Where(u => u.Type == "Fullname").FirstOrDefault()?.Value;
            if (bool.TryParse(claims.Where(u => u.Type == "IsCompany").FirstOrDefault()?.Value, out bool _b))
            {
                a.IsCompany = _b;
            }
            else
            {
                a.IsCompany = false;
            }
            if (DateTime.TryParse(claims.Where(u => u.Type == "IssueDate").FirstOrDefault()?.Value, out DateTime _c))
            {
                a.IssueDate = _c;
            }
            else
            {
                a.IssueDate = default;
            }
            a.IssuePlace = claims.Where(u => u.Type == "IssuePlace").FirstOrDefault()?.Value;
            a.PersonID = claims.Where(u => u.Type == "PersonID").FirstOrDefault()?.Value;
            a.PhoneNumber = claims.Where(u => u.Type == "PhoneNumber").FirstOrDefault()?.Value;
            _logger.LogInformation($"Get profile: {a1.UserName} => {JsonConvert.SerializeObject(a)}");
            switch (profileType)
            {
                case 1:
                    return new ResponseOK()
                    {
                        Code = 1,
                        data = a,
                        InternalMessage = LanguageAll.Language.GetProfileOK,
                        MoreInfo = LanguageAll.Language.GetProfileOK,
                        Status = 200,
                        UserMessage = LanguageAll.Language.GetProfileOK
                    };
                case 2:
                    return new ResponseOK()
                    {
                        Code = 200,
                        InternalMessage = LanguageAll.Language.SetProfileSuccess,
                        MoreInfo = LanguageAll.Language.SetProfileSuccess,
                        Status = 1,
                        UserMessage = LanguageAll.Language.SetProfileSuccess,
                        data = a
                    };
                case 3:
                    return new ResponseOK()
                    {
                        Code = 1,
                        data = a,
                        InternalMessage = LanguageAll.Language.LinkInvoiceSuccess,
                        MoreInfo = LanguageAll.Language.LinkInvoiceSuccess,
                        Status = 200,
                        UserMessage = LanguageAll.Language.LinkInvoiceSuccess
                    };
                case 4:
                    return new ResponseOK()
                    {
                        Code = 1,
                        data = a,
                        InternalMessage = LanguageAll.Language.RemoveInvoiceSuccess,
                        MoreInfo = LanguageAll.Language.RemoveInvoiceSuccess,
                        Status = 200,
                        UserMessage = LanguageAll.Language.RemoveInvoiceSuccess
                    };
                default:
                    return new ResponseOK()
                    {
                        Code = 1,
                        data = a,
                        InternalMessage = LanguageAll.Language.SetCompanyInfoSuccess,
                        MoreInfo = LanguageAll.Language.SetCompanyInfoSuccess,
                        Status = 200,
                        UserMessage = LanguageAll.Language.SetCompanyInfoSuccess
                    };
            }
        }

        public async Task<ResponseOK> SetProfile(ProfileInputModel inv)
        {
            if (!String.IsNullOrEmpty(inv.Email))
            {
                var a1 = await _userManager.FindByEmailAsync(inv.Email);
                if (a1 == null)
                {
                    a1 = await _userManager.GetUserAsync(_context.HttpContext.User);
                    a1.Email = inv.Email;
                    await _userManager.UpdateAsync(a1);
                }
                else
                {
                    return new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.SetProfileFailEmail,
                        MoreInfo = LanguageAll.Language.SetProfileFailEmail,
                        Status = 0,
                        UserMessage = LanguageAll.Language.SetProfileFailEmail,
                        data = null
                    };
                }
            }

            if (String.IsNullOrEmpty(inv.CompanyName) && inv.IsCompany)
            {
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.SetProfileFailCompanyName,
                    MoreInfo = LanguageAll.Language.SetProfileFailCompanyName,
                    Status = 0,
                    UserMessage = LanguageAll.Language.SetProfileFailCompanyName,
                    data = null
                };
            }

            if (String.IsNullOrEmpty(inv.Fullname))
            {
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.SetProfileFailFullname,
                    MoreInfo = LanguageAll.Language.SetProfileFailFullname,
                    Status = 0,
                    UserMessage = LanguageAll.Language.SetProfileFailFullname,
                    data = null
                };
            }

            var u = await _userManager.GetUserAsync(_context.HttpContext.User);
            var a = await _userManager.GetClaimsAsync(u);
            if (!String.IsNullOrEmpty(inv.Address))
            {
                if (String.IsNullOrEmpty(a.Where(u => u.Type == "Address").FirstOrDefault()?.Value))
                {
                    await _userManager.AddClaimAsync(u, new Claim("Address", inv.Address));
                }
                else
                {
                    await _userManager.ReplaceClaimAsync(u, a.Where(u => u.Type == "Address").FirstOrDefault(), new Claim("Address", inv.Address));
                }
            }
            if (!String.IsNullOrEmpty(inv.CompanyName))
            {
                if (String.IsNullOrEmpty(a.Where(u => u.Type == "CompanyName").FirstOrDefault()?.Value))
                {
                    await _userManager.AddClaimAsync(u, new Claim("CompanyName", inv.CompanyName));
                }
                else
                {
                    await _userManager.ReplaceClaimAsync(u, a.Where(u => u.Type == "CompanyName").FirstOrDefault(), new Claim("CompanyName", inv.CompanyName));
                }
            }
            if (String.IsNullOrEmpty(a.Where(u => u.Type == "Fullname").FirstOrDefault()?.Value))
            {
                await _userManager.AddClaimAsync(u, new Claim("Fullname", inv.Fullname));
            }
            else
            {
                await _userManager.ReplaceClaimAsync(u, a.Where(u => u.Type == "Fullname").FirstOrDefault(), new Claim("Fullname", inv.Fullname));
            }
            if (String.IsNullOrEmpty(a.Where(u => u.Type == "IsCompany").FirstOrDefault()?.Value))
            {
                await _userManager.AddClaimAsync(u, new Claim("IsCompany", inv.IsCompany.ToString()));
            }
            else
            {
                await _userManager.ReplaceClaimAsync(u, a.Where(u => u.Type == "IsCompany").FirstOrDefault(), new Claim("IsCompany", inv.IsCompany.ToString()));
            }
            if (inv.IssueDate.HasValue)
            {
                if (String.IsNullOrEmpty(a.Where(u => u.Type == "IssueDate").FirstOrDefault()?.Value))
                {
                    await _userManager.AddClaimAsync(u, new Claim("IssueDate", inv.IssueDate.ToString()));
                }
                else
                {
                    await _userManager.ReplaceClaimAsync(u, a.Where(u => u.Type == "IssueDate").FirstOrDefault(), new Claim("IssueDate", inv.IssueDate.ToString()));
                }
            }
            if (!String.IsNullOrEmpty(inv.IssuePlace))
            {
                if (String.IsNullOrEmpty(a.Where(u => u.Type == "IssuePlace").FirstOrDefault()?.Value))
                {
                    await _userManager.AddClaimAsync(u, new Claim("IssuePlace", inv.IssuePlace));
                }
                else
                {
                    await _userManager.ReplaceClaimAsync(u, a.Where(u => u.Type == "IssuePlace").FirstOrDefault(), new Claim("IssuePlace", inv.IssuePlace));
                }
            }
            if (!String.IsNullOrEmpty(inv.PersonID))
            {
                if (String.IsNullOrEmpty(a.Where(u => u.Type == "PersonID").FirstOrDefault()?.Value))
                {
                    await _userManager.AddClaimAsync(u, new Claim("PersonID", inv.PersonID));
                }
                else
                {
                    await _userManager.ReplaceClaimAsync(u, a.Where(u => u.Type == "PersonID").FirstOrDefault(), new Claim("PersonID", inv.PersonID));
                }
            }
            if (!String.IsNullOrEmpty(inv.PhoneNumber))
            {
                if (String.IsNullOrEmpty(a.Where(u => u.Type == "PhoneNumber").FirstOrDefault()?.Value))
                {
                    await _userManager.AddClaimAsync(u, new Claim("PhoneNumber", inv.PhoneNumber));
                }
                else
                {
                    await _userManager.ReplaceClaimAsync(u, a.Where(u => u.Type == "PhoneNumber").FirstOrDefault(), new Claim("PhoneNumber", inv.PhoneNumber));
                }
            }
            return await GetProfile(2);
        }

        public async Task<ResponseOK> LinkInvoice(InvoiceInput inv)
        {
            if (String.IsNullOrEmpty(inv.CustomerCode))
            {
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.LinkInvoiceFailCodeEmpty,
                    MoreInfo = LanguageAll.Language.LinkInvoiceFailCodeEmpty,
                    Status = 0,
                    UserMessage = LanguageAll.Language.LinkInvoiceFailCodeEmpty,
                    data = null
                };
            }

            InvoiceResult ir = await _invoice.GetInvoice(inv);
            if (ir.DataStatus != "01" && ir.DataStatus != "00")
            {
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.LinkInvoiceFailCodeNotExitst,
                    MoreInfo = LanguageAll.Language.LinkInvoiceFailCodeNotExitst,
                    Status = 0,
                    UserMessage = LanguageAll.Language.LinkInvoiceFailCodeNotExitst,
                    data = null
                };
            }

            var _claim = new Claim("GetInvoice", inv.CustomerCode);
            var _u = await _userManager.GetUsersForClaimAsync(_claim);
            if (_u.Count == 0)
            {
                var u = await _userManager.GetUserAsync(_context.HttpContext.User);
                await _userManager.AddClaimAsync(u, _claim);
                return await GetProfile(3);
            }

            return new ResponseOK()
            {
                Code = 400,
                InternalMessage = LanguageAll.Language.LinkInvoiceFailCodeHasLink,
                MoreInfo = LanguageAll.Language.LinkInvoiceFailCodeHasLink,
                Status = 0,
                UserMessage = LanguageAll.Language.LinkInvoiceFailCodeHasLink,
                data = null
            };
        }

        public async Task<ResponseOK> RemoveInvoice(InvoiceInput inv)
        {
            if (String.IsNullOrEmpty(inv.CustomerCode))
            {
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.LinkInvoiceFailCodeEmpty,
                    MoreInfo = LanguageAll.Language.LinkInvoiceFailCodeEmpty,
                    Status = 0,
                    UserMessage = LanguageAll.Language.LinkInvoiceFailCodeEmpty,
                    data = null
                };
            }

            InvoiceResult ir = await _invoice.GetInvoice(inv);
            if (ir.DataStatus != "01" && ir.DataStatus != "00")
            {
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.LinkInvoiceFailCodeNotExitst,
                    MoreInfo = LanguageAll.Language.LinkInvoiceFailCodeNotExitst,
                    Status = 0,
                    UserMessage = LanguageAll.Language.LinkInvoiceFailCodeNotExitst,
                    data = null
                };
            }

            var u = await _userManager.GetUserAsync(_context.HttpContext.User);
            var a = await _userManager.GetClaimsAsync(u);
            if (String.IsNullOrEmpty(a.Where(u => u.Value == inv.CustomerCode).FirstOrDefault()?.Value))
            {
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.LinkInvoiceFailCodeHasLink,
                    MoreInfo = LanguageAll.Language.LinkInvoiceFailCodeHasLink,
                    Status = 0,
                    UserMessage = LanguageAll.Language.LinkInvoiceFailCodeHasLink,
                    data = null
                };
            }

            await _userManager.RemoveClaimAsync(u, new Claim("GetInvoice", inv.CustomerCode));

            return await GetProfile(4);
        }

        public async Task<ResponseOK> GetCompanyList()
        {
            var a1 = await _userManager.GetUserAsync(_context.HttpContext.User);
            _logger.LogInformation($"GetCompanyList: {a1.UserName}");
            var r = new List<CompanyInfo>();
            foreach (var item in _invoice.companyConfig.Companys)
            {
                r.Add(new CompanyInfo()
                {
                    CompanyCode = item.Info.CompanyCode,
                    CompanyName = item.Info.CompanyName,
                    CompanyLogo = item.Info.CompanyLogo,
                    CompanyNameEn = item.Info.CompanyNameEn
                });
            }
            return new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = r
            };
        }

        public async Task<ResponseOK> SetCompanyInfo(CompanyInfoInput inv)
        {
            var a1 = await _userManager.GetUserAsync(_context.HttpContext.User);
            _logger.LogInformation($"SetCompanyInfo: {a1.UserName}");
            if (String.IsNullOrEmpty(inv.CompanyCode))
            {
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.SetProfileFailFullname,
                    MoreInfo = LanguageAll.Language.SetProfileFailFullname,
                    Status = 0,
                    UserMessage = LanguageAll.Language.SetProfileFailFullname,
                    data = null
                };
            }
            var u = await _userManager.GetUserAsync(_context.HttpContext.User);
            var a = await _userManager.GetClaimsAsync(u);
            var c = a.Where(u => u.Type == "WaterCompany").FirstOrDefault();
            int CompanyId = 0;
            bool Found = false;
            while (!Found && CompanyId < _invoice.companyConfig.Companys.Count)
            {
                if (_invoice.companyConfig.Companys[CompanyId].Info.CompanyCode == inv.CompanyCode) Found = true;
                CompanyId++;
            }
            if (Found)
                CompanyId = CompanyId - 1;
            else
                CompanyId = 0;
            var c1 = new Claim("WaterCompany", CompanyId.ToString());
            if (c == default || c == null)
            {
                await _userManager.AddClaimAsync(u, c1);
            }
            else
            {
                await _userManager.ReplaceClaimAsync(u, c, c1);
            }
            return await GetProfile(5);
        }

        public async Task<ResponseOK> GetCompanyInfo(int profileType = 1)
        {
            var a1 = await _userManager.GetUserAsync(_context.HttpContext.User);
            _logger.LogInformation($"GetCompanyInfo: {a1.UserName}");
            IEnumerable<Claim> claims;
            if (profileType == 1)
            {
                claims = _context.HttpContext.User.Claims;
            }
            else
            {
                claims = await _userManager.GetClaimsAsync(a1);
            }

            int CompanyID = 0;
            if (!int.TryParse(claims.Where(u => u.Type == "WaterCompany").FirstOrDefault()?.Value, out CompanyID))
            {
                CompanyID = 0;
            }

            var r = new CompanyInfo()
            {
                CompanyCode = _invoice.companyConfig.Companys[CompanyID].Info.CompanyCode,
                CompanyName = _invoice.companyConfig.Companys[CompanyID].Info.CompanyName,
                CompanyLogo = _invoice.companyConfig.Companys[CompanyID].Info.CompanyLogo,
                CompanyNameEn = _invoice.companyConfig.Companys[CompanyID].Info.CompanyNameEn
            };
            return new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = r
            };
        }
    }
}
