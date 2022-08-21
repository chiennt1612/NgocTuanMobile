using Auth.Helper;
using Auth.Models;
using Auth.Services.Interfaces;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    //[Authorize]
    public class AboutController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<AboutController> _logger;
        private readonly IAllService _Service;
        private readonly IConfiguration _configuration;

        public AboutController(IDistributedCache cache, ILogger<AboutController> logger, IAllService Service, IConfiguration configuration)
        {
            this._logger = logger;
            this._Service = Service;
            this._cache = cache;
            this._configuration = configuration;
            _logger.WriteLog("Starting about page");
        }

        [HttpGet]
        [Route("[action]/{language}")]
        public async Task<IActionResult> ExtendList(string language = "Vi")
        {
            List<AboutModel> r = await _cache.GetAsync<List<AboutModel>>($"ExtendList_{language}");
            if (r == null)
            {
                r = new List<AboutModel>();
                var _guide = _configuration.GetSection(nameof(AboutPage)).Get<AboutPage>();
                var a = language == "Vi" ? _guide.ExtendID.Vi : _guide.ExtendID.En;
                Expression<Func<About, bool>> expression = u =>  a.Contains(u.Id);
                var items = await _Service.aboutServices.GetManyAsync(expression);
                int i = 0;
                foreach(var b in items)
                {
                    r.Add(new AboutModel()
                    {
                        Id = b.Id,
                        //Description = b.Description,
                        Url = Tools.GetUrlById("About", b.Id),
                        Sort = i++,
                        Title = b.Title
                    });
                }
                await _cache.SetAsync<List<AboutModel>>($"ExtendList_{language}", r);
            }
            _logger.WriteLog($"ExtendList: ", "ExtendList");
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = r
            });
        }
        [HttpGet]
        [Route("[action]/{language}")]
        public async Task<IActionResult> GuideList(string language = "Vi")
        {
            List<AboutModel> r = await _cache.GetAsync<List<AboutModel>>($"GuideList_{language}");
            if (r == null)
            {
                r = new List<AboutModel>();
                var _guide = _configuration.GetSection(nameof(AboutPage)).Get<AboutPage>();
                var a = language == "Vi" ? _guide.GuideID.Vi : _guide.GuideID.En;
                Expression<Func<About, bool>> expression = u => a.Contains(u.Id);
                var items = (await _Service.aboutServices.GetManyAsync(expression)).OrderBy(u => u.Title);
                int i = 0;
                foreach (var b in items)
                {
                    r.Add(new AboutModel()
                    {
                        Id = b.Id,
                        //Description = b.Description,
                        Url = Tools.GetUrlById("About", b.Id),
                        Sort = i++,
                        Title = b.Title
                    });
                }
                await _cache.SetAsync<List<AboutModel>>($"GuideList_{language}", r);
            }
            _logger.WriteLog($"GuideList: ", "GuideList");
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = r
            });
        }

        [HttpGet]
        [Route("[action]/{language}")]
        public async Task<IActionResult> AboutPage(string language = "Vi")
        {
            AboutModel r = await _cache.GetAsync<AboutModel>($"AboutPage_{language}");
            if (r == null)
            {
                var _guide = _configuration.GetSection(nameof(AboutPage)).Get<AboutPage>();
                var b = await _Service.aboutServices.GetByIdAsync(language == "Vi" ? _guide.AboutID.Vi : _guide.AboutID.En);
                r = new AboutModel()
                {
                    Id = b.Id,
                    Description = b.Description,
                    Url = Tools.GetUrlById("About", b.Id),
                    Sort = 0,
                    Title = b.Title
                };
                await _cache.SetAsync<AboutModel>($"AboutPage_{language}", r);
            }
            _logger.WriteLog($"About: ", "About");
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = r
            });
        }

        [HttpGet]
        [Route("[action]/{pageId}")]
        public async Task<IActionResult> PageInfo(long pageId)
        {
            AboutModel r = await _cache.GetAsync<AboutModel>($"PageInfo_{pageId}");
            if (r == null)
            {
                var b = await _Service.aboutServices.GetByIdAsync(pageId);
                r = new AboutModel()
                {
                    Id = b.Id,
                    Description = b.Description,
                    Url = Tools.GetUrlById("About", b.Id),
                    Sort = 0,
                    Title = b.Title
                };
                await _cache.SetAsync<AboutModel>($"PageInfo_{pageId}", r);
            }
            _logger.WriteLog($"PageInfo {pageId}: ", $"PageInfo {pageId}");
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = r
            });
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Advs()
        {
            List<AdvModel> r = await _cache.GetAsync<List<AdvModel>>($"Advs");//IEnumerable
            if (r == null)
            {
                r = (from p in (await _Service.advServices.GetAllAsync())
                     select new AdvModel()
                     {
                         AdvScript = p.AdvScript,
                         CustomerName = p.CustomerName,
                         Id = p.Id,
                         Img = p.Img,
                         Position = p.AdvPosition,
                         Website = p.Website
                     }).ToList();
                await _cache.SetAsync<List<AdvModel>>($"Advs", r);
            }
            _logger.WriteLog($"Advs: ", $"Advs");
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = r
            });
        }
    }
}
