using Auth.Models;
using Auth.Repository.Interfaces;
using Auth.Services.Interfaces;
using EntityFramework.API.Entities;
using EntityFramework.API.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils;
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
        private readonly IContractServices _contract;
        private readonly IUserDeviceRepository _iUserDeviceRepository;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private List<string> invoiceCacheKeys;

        public Profile(
            ILogger<Profile> logger,
            UserManager<AppUser> userManager,
            IInvoiceRepository invoice,
            IHttpContextAccessor context,
            IUserDeviceRepository iUserDeviceRepository,
            IContractServices contract,
            IDistributedCache cache,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _invoice = invoice;
            _iUserDeviceRepository = iUserDeviceRepository;
            _contract = contract;
            _cache = cache;
            _configuration = configuration;

            var b = _cache.GetAsync<List<string>>("InvoiceCacheKeys").GetAwaiter();
            invoiceCacheKeys = b.GetResult();
            if (invoiceCacheKeys == null)
            {
                invoiceCacheKeys = new List<string>();
            }
        }

        public async Task<ResponseOK> GetProfile(int profileType = 1)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            var a1 = await _userManager.GetUserAsync(_context.HttpContext.User);
            _logger.LogInformation($"Get profile: {a1.UserName}");
            IEnumerable<Claim> claims;
            string DeviceId = _context.HttpContext.User.Claims.Where(u => u.Type == "DeviceId").FirstOrDefault()?.Value;
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

            Expression<Func<AppUserDevice, bool>> sqlWhere = u => (u.DeviceID == DeviceId);
            var UserByDevice = await _iUserDeviceRepository.GetAsync(sqlWhere);
            a.IsGetNotice = UserByDevice != null ? UserByDevice.IsGetNotice : false;
            a.DeviceId = DeviceId;

            a.CompanyName = claims.Where(u => u.Type == "CompanyName").FirstOrDefault()?.Value;
            a.WaterCompany = claims.Where(u => u.Type == "WaterCompany").FirstOrDefault()?.Value;
            foreach (var item in claims.Where(u => u.Type == "GetInvoice"))
            {
                a.CustomerCodeList.Add(item.Value);
            }
            a.Email = a1.Email;// claims.Where(u => u.Type == ClaimTypes.Email).FirstOrDefault()?.Value;
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
            a.Avatar = claims.Where(u => u.Type == "Avatar").FirstOrDefault()?.Value;
            a.IssuePlace = claims.Where(u => u.Type == "IssuePlace").FirstOrDefault()?.Value;
            a.PersonID = claims.Where(u => u.Type == "PersonID").FirstOrDefault()?.Value;
            a.PhoneNumber = a1.UserName;// claims.Where(u => u.Type == "PhoneNumber").FirstOrDefault()?.Value;
            _logger.LogInformation($"Get profile: {a1.UserName} => {a.Fullname}");
            switch (profileType)
            {
                case 1:
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return new ResponseOK()
                    {
                        Code = 200,
                        data = a,
                        InternalMessage = LanguageAll.Language.GetProfileOK,
                        MoreInfo = LanguageAll.Language.GetProfileOK,
                        Status = 1,
                        UserMessage = LanguageAll.Language.GetProfileOK
                    };
                case 2:
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return new ResponseOK()
                    {
                        Code = 200,
                        data = a,
                        InternalMessage = LanguageAll.Language.LinkInvoiceSuccess,
                        MoreInfo = LanguageAll.Language.LinkInvoiceSuccess,
                        Status = 1,
                        UserMessage = LanguageAll.Language.LinkInvoiceSuccess
                    };
                case 4:
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return new ResponseOK()
                    {
                        Code = 200,
                        data = a,
                        InternalMessage = LanguageAll.Language.RemoveInvoiceSuccess,
                        MoreInfo = LanguageAll.Language.RemoveInvoiceSuccess,
                        Status = 1,
                        UserMessage = LanguageAll.Language.RemoveInvoiceSuccess
                    };
                default:
                    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                    return new ResponseOK()
                    {
                        Code = 200,
                        data = a,
                        InternalMessage = LanguageAll.Language.SetCompanyInfoSuccess,
                        MoreInfo = LanguageAll.Language.SetCompanyInfoSuccess,
                        Status = 1,
                        UserMessage = LanguageAll.Language.SetCompanyInfoSuccess
                    };
            }
        }

        public async Task<ResponseOK> SetProfile(ProfileInputModel inv)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"inv: {JsonConvert.SerializeObject(inv)}");
            if (inv.Email.IsValidEmail())
            {
                var a1 = await _userManager.FindByEmailAsync(inv.Email);
                if (a1 == null)
                {
                    a1 = await _userManager.GetUserAsync(_context.HttpContext.User);
                    a1.Email = inv.Email;
                    await _userManager.UpdateAsync(a1);
                }
                //else
                //{
                //    return new ResponseOK()
                //    {
                //        Code = 400,
                //        InternalMessage = LanguageAll.Language.SetProfileFailEmail,
                //        MoreInfo = LanguageAll.Language.SetProfileFailEmail,
                //        Status = 0,
                //        UserMessage = LanguageAll.Language.SetProfileFailEmail,
                //        data = null
                //    };
                //}
            }

            if (String.IsNullOrEmpty(inv.CompanyName) && inv.IsCompany)
            {
                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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
                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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
            if (!string.IsNullOrEmpty(inv.Avatar))
            {
                if (a.Where(u => u.Type == "Avatar").FirstOrDefault() != null)
                {
                    _logger.WriteLog(_configuration, $"4.1. SetProfile {inv.Avatar}");
                    await _userManager.RemoveClaimAsync(u, new Claim("Avatar", inv.Avatar));
                }
                await _userManager.AddClaimAsync(u, new Claim("Avatar", inv.Avatar));
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
            //if (!String.IsNullOrEmpty(inv.PhoneNumber))
            //{
            //    if (String.IsNullOrEmpty(a.Where(u => u.Type == "PhoneNumber").FirstOrDefault()?.Value))
            //    {
            //        await _userManager.AddClaimAsync(u, new Claim("PhoneNumber", inv.PhoneNumber));
            //    }
            //    else
            //    {
            //        await _userManager.ReplaceClaimAsync(u, a.Where(u => u.Type == "PhoneNumber").FirstOrDefault(), new Claim("PhoneNumber", inv.PhoneNumber));
            //    }
            //}
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return await GetProfile(2);
        }

        public async Task<ResponseOK> LinkInvoice(InvoiceInput inv)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"inv: {JsonConvert.SerializeObject(inv)}");
            if (inv.CompanyID < 0 || inv.CompanyID > 100)
            {
                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.CompanyIDInvalid,
                    MoreInfo = LanguageAll.Language.CompanyIDInvalid,
                    Status = 0,
                    UserMessage = LanguageAll.Language.CompanyIDInvalid,
                    data = null
                };
            }
            if (String.IsNullOrEmpty(inv.CustomerCode))
            {
                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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

            EVNCodeInput inv1 = new EVNCodeInput()
            {
                CompanyID = inv.CompanyID,
                EVNCode = inv.CustomerCode
            };
            CustomerInfoResult ir = await _invoice.getCustomerInfo(inv1);
            if (ir.DataStatus != "00")
            {
                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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

            var _claim = new Claim("GetInvoice", $"{inv.CompanyID}.{inv.CustomerCode}");
            //var _u = await _userManager.GetUsersForClaimAsync(_claim);
            //if (_u.Count == 0)
            //{
                var u = await _userManager.GetUserAsync(_context.HttpContext.User);
                await _contract.AddAsync(new Contract()
                {
                    Address = ir.ItemsData[0].Address,
                    CompanyId = inv.CompanyID,
                    CustomerCode = ir.ItemsData[0].CustomerCode,
                    CustomerName = ir.ItemsData[0].CustomerName,
                    CustomerType = ir.ItemsData[0].CustomerType,
                    Email = ir.ItemsData[0].Email,
                    Mobile = ir.ItemsData[0].Mobile,
                    TaxCode = ir.ItemsData[0].TaxCode,
                    UserId = u.Id,
                    WaterIndexCode = ir.ItemsData[0].WaterIndexCode
                });

                await _userManager.AddClaimAsync(u, _claim);
                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                return await GetProfile(3);
            //}
            //_logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            //return new ResponseOK()
            //{
            //    Code = 400,
            //    InternalMessage = LanguageAll.Language.LinkInvoiceFailCodeHasLink,
            //    MoreInfo = LanguageAll.Language.LinkInvoiceFailCodeHasLink,
            //    Status = 0,
            //    UserMessage = LanguageAll.Language.LinkInvoiceFailCodeHasLink,
            //    data = null
            //};
        }

        public async Task<ResponseOK> RemoveInvoice(InvoiceInput inv)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"inv: {JsonConvert.SerializeObject(inv)}");
            if (inv.CompanyID < 0 || inv.CompanyID > 100)
            {
                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.CompanyIDInvalid,
                    MoreInfo = LanguageAll.Language.CompanyIDInvalid,
                    Status = 0,
                    UserMessage = LanguageAll.Language.CompanyIDInvalid,
                    data = null
                };
            }
            if (String.IsNullOrEmpty(inv.CustomerCode))
            {
                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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

            //InvoiceResult ir = await _invoice.GetInvoice(inv);
            //if (ir.DataStatus != "01" && ir.DataStatus != "00")
            //{
            //    return new ResponseOK()
            //    {
            //        Code = 400,
            //        InternalMessage = LanguageAll.Language.LinkInvoiceFailCodeNotExitst,
            //        MoreInfo = LanguageAll.Language.LinkInvoiceFailCodeNotExitst,
            //        Status = 0,
            //        UserMessage = LanguageAll.Language.LinkInvoiceFailCodeNotExitst,
            //        data = null
            //    };
            //}

            var _claim = new Claim("GetInvoice", $"{inv.CompanyID}.{inv.CustomerCode}");
            var u = await _userManager.GetUserAsync(_context.HttpContext.User);
            var a = await _userManager.GetClaimsAsync(u);
            //if (a.Where(u => u.Type == _claim.Type && u.Value == _claim.Value).FirstOrDefault() == default)
            //{
            //    _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            //    return new ResponseOK()
            //    {
            //        Code = 400,
            //        InternalMessage = LanguageAll.Language.NotFound,
            //        MoreInfo = LanguageAll.Language.NotFound,
            //        Status = 0,
            //        UserMessage = LanguageAll.Language.NotFound,
            //        data = null
            //    };
            //}

            Expression<Func<Contract, bool>> expression = u => (
                    (u.CompanyId >= inv.CompanyID) &&
                    (u.CustomerCode == inv.CustomerCode));
            var contract = await _contract.GetAsync(expression);
            _logger.LogInformation($"inv: {contract.Id} | {inv.CustomerCode} | {inv.CompanyID} | {inv.CompanyID}.{inv.CustomerCode}");
            if (contract != null)
            {
                await _contract.DeleteAsync(contract);
                await _userManager.RemoveClaimAsync(u, _claim);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return await GetProfile(4);
        }

        public async Task<ResponseOK> GetCompanyList()
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
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
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            var a1 = await _userManager.GetUserAsync(_context.HttpContext.User);
            _logger.LogInformation($"SetCompanyInfo: {a1.UserName}");
            if (String.IsNullOrEmpty(inv.CompanyCode))
            {
                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return await GetProfile(5);
        }

        public async Task<ResponseOK> GetCompanyInfo(int profileType = 1)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
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
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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

        public async Task<ResponseOK> GetContractAllList(ContractInput inv)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            var _company = _invoice.companyConfig.Companys.Where(u => u.Info.CompanyId == inv.CompanyID).FirstOrDefault();
            if (_company == null)
            {
                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.CompanyIDInvalid,
                    MoreInfo = LanguageAll.Language.CompanyIDInvalid,
                    Status = 0,
                    UserMessage = LanguageAll.Language.CompanyIDInvalid,
                    data = null
                };
            }
            if (String.IsNullOrEmpty(inv.Mobile))
            {
                _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.FormatPhoneNumberFail,
                    MoreInfo = LanguageAll.Language.FormatPhoneNumberFail,
                    Status = 0,
                    UserMessage = LanguageAll.Language.FormatPhoneNumberFail,
                    data = null
                };
            }

            string _key = $"GetContractAllList.{inv.CompanyID}.{inv.Mobile}".ToMD5Hash();
            if (!invoiceCacheKeys.Contains(_key))
            {
                invoiceCacheKeys.Add(_key);
                await _cache.InvoiceSetAsync("InvoiceCacheKeys", invoiceCacheKeys);
            }
            ContractResult ir = await _cache.GetAsync<ContractResult>(_key);
            if (ir == null)
            {
                ir = await _invoice.GetContract(inv);
                ir.CompanyInfo = _company.Info;
                await _cache.InvoiceSetAsync(_key, ir);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 0,
                UserMessage = LanguageAll.Language.Success,
                data = ir
            };
        }

        public async Task<ContractResult> GetContractList(ContractInput inv)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            string _key = $"GetContractList.{inv.CompanyID}.{inv.Mobile}".ToMD5Hash();
            if (!invoiceCacheKeys.Contains(_key))
            {
                invoiceCacheKeys.Add(_key);
                await _cache.InvoiceSetAsync("InvoiceCacheKeys", invoiceCacheKeys);
            }
            ContractResult a = await _cache.GetAsync<ContractResult>(_key);
            if (a == null)
            {
                a = await _invoice.GetContract(inv);
                await _cache.InvoiceSetAsync(_key, a);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return a;
        }
    }
}
