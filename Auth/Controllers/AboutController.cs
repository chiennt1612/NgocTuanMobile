using Auth.Helper;
using Auth.Models;
using Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
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
        public async Task<IActionResult> GuideList(string language = "Vi")
        {
            List<AboutModel> r = await _cache.GetAsync<List<AboutModel>>($"GuideList_{language}");
            if (r == null)
            {
                var _guide = _configuration.GetSection(nameof(AboutPage)).Get<AboutPage>();
                var a = language == "Vi" ? _guide.GuideID.Vi : _guide.GuideID.En;
                var i = 0;
                r = (from b in ((await _Service.aboutServices.GetAllAsync()).Where(u => a.Contains(u.Id)).OrderBy(u => u.Title))
                     select new AboutModel()
                     {
                         Id = b.Id,
                         Description = b.Description,
                         Sort = i++,
                         Title = b.Title
                     }).ToList();
                await _cache.SetAsync<List<AboutModel>>($"GuideList_{language}", r);
            }
            _logger.WriteLog($"GuideList: {JsonConvert.SerializeObject(r)}", "GuideList");
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
                    Sort = 0,
                    Title = b.Title
                };
                await _cache.SetAsync<AboutModel>($"AboutPage_{language}", r);
            }
            _logger.WriteLog($"About: {JsonConvert.SerializeObject(r)}", "About");
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
                    Sort = 0,
                    Title = b.Title
                };
                await _cache.SetAsync<AboutModel>($"PageInfo_{pageId}", r);
            }
            _logger.WriteLog($"PageInfo {pageId}: {JsonConvert.SerializeObject(r)}", $"PageInfo {pageId}");
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
            _logger.WriteLog($"Advs: {JsonConvert.SerializeObject(r)}", $"Advs");
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
