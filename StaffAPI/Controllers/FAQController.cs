using StaffAPI.Models;
using StaffAPI.Services.Interfaces;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Utils;
using Utils.ExceptionHandling;
using Utils.Models;

namespace StaffAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("[controller]")]
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
            _logger.WriteLog(_configuration, "Starting FAQ page");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> List(int? page, int? pageSize)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            int _Page = (page.HasValue ? page.Value : 1);
            int _PageSize = pageSize.HasValue ? pageSize.Value : 10;
            Func<FAQ, object> sqlOrder = s => s.Id;
            Expression<Func<FAQ, bool>> sqlWhere = u => (true);
            var a = await _Service.fAQServices.GetListAsync(sqlWhere, sqlOrder, true, _Page, _PageSize);
            _logger.WriteLog(_configuration, $"List FAQ {page}/{pageSize}", $"List FAQ {page}/{pageSize}");
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            Expression<Func<Article, bool>> sqlWhereNew = u => (u.IsNews);
            var article = await _Service.fAQServices.GetByIdAsync(Id);
            Expression<Func<FAQ, bool>> sqlWhere = u => (u.Id < Id);
            var a = new FAQModel()
            {
                fAQ = article,
                fAQs = await _Service.fAQServices.GetTopAsync(sqlWhere, 10)
            };
            _logger.WriteLog(_configuration, $"Details {Id}", $"Details {Id}");
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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
