using Auth.Models;
using Auth.Services.Interfaces;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
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
    [Authorize]
    public class FAQController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<FAQController> _logger;
        private readonly IAllService _Service;
        private readonly IConfiguration _configuration;

        public FAQController(IDistributedCache cache, ILogger<FAQController> logger, IAllService Service, IConfiguration configuration)
        {
            this._logger = logger;
            this._Service = Service;
            this._cache = cache;
            this._configuration = configuration;
            _logger.WriteLog("Starting FAQ page");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> List(int? page, int? pageSize)
        {
            int _Page = (page.HasValue ? page.Value : 1);
            int _PageSize = pageSize.HasValue ? pageSize.Value : 10;
            Func<FAQ, object> sqlOrder = s => s.Id;
            Expression<Func<FAQ, bool>> sqlWhere = u => (true);
            var a = await _Service.fAQServices.GetListAsync(sqlWhere, sqlOrder, true, _Page, _PageSize);
            _logger.WriteLog($"List FAQ {page}/{pageSize}", $"List FAQ {page}/{pageSize}");
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = a
            });
        }

        [HttpGet]
        [Route("[action]/{Id}")]
        public async Task<IActionResult> Details(long Id)
        {
            Expression<Func<Article, bool>> sqlWhereNew = u => (u.IsNews);
            var article = await _Service.fAQServices.GetByIdAsync(Id);
            Expression<Func<FAQ, bool>> sqlWhere = u => (u.Id < Id);
            var a = new FAQModel()
            {
                fAQ = article,
                fAQs = await _Service.fAQServices.GetTopAsync(sqlWhere, 10)
            };
            _logger.WriteLog($"Details {Id}", $"Details {Id}");
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = a
            });
        }
    }
}
