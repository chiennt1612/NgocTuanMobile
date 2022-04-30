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

namespace Auth.Services
{
    public class Profile : IProfile
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<Profile> _logger;
        private readonly IHttpContextAccessor _context;

        public Profile(
            ILogger<Profile> logger,
            UserManager<AppUser> userManager,
            IHttpContextAccessor context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task<ResponseOK> GetProfile(bool IsFromToken = true)
        {
            var a1 = await _userManager.GetUserAsync(_context.HttpContext.User);
            _logger.LogInformation($"Get profile: {a1.UserName}");
            IEnumerable<Claim> claims;
            if (IsFromToken)
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
            a.CustomerCode = claims.Where(u => u.Type == "CustomerCode").FirstOrDefault()?.Value;
            foreach (var item in _context.HttpContext.User.FindAll("GetInvoice"))
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
            if (IsFromToken)
            {
                return new ResponseOK()
                {
                    Code = 1,
                    data = a,
                    InternalMessage = LanguageAll.Language.GetProfileOK,
                    MoreInfo = LanguageAll.Language.GetProfileOK,
                    Status = 200,
                    UserMessage = LanguageAll.Language.GetProfileOK
                };
            }
            else
            {
                return new ResponseOK()
                {
                    Code = 200,
                    InternalMessage = LanguageAll.Language.SetProfileSuccess,
                    MoreInfo = LanguageAll.Language.SetProfileSuccess,
                    Status = 1,
                    UserMessage = LanguageAll.Language.SetProfileSuccess,
                    data = a
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
            return await GetProfile(false);
            //    new ResponseOK()
            //{
            //    Code = 200,
            //    InternalMessage = LanguageAll.Language.SetProfileSuccess,
            //    MoreInfo = LanguageAll.Language.SetProfileSuccess,
            //    Status = 1,
            //    UserMessage = LanguageAll.Language.SetProfileSuccess,
            //    data = null
            //};
        }
    }
}
