﻿using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StaffAPI.Models.News;
using StaffAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class NewsController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<NewsController> _logger;
        private readonly IAllService _Service;
        private readonly IConfiguration _configuration;

        public NewsController(IDistributedCache cache, ILogger<NewsController> logger, IAllService Service, IConfiguration configuration)
        {
            this._logger = logger;
            this._Service = Service;
            this._cache = cache;
            this._configuration = configuration;
            _logger.WriteLog(_configuration, "Starting news page");
        }

        [HttpGet]
        [Route("[action]/{language}")]
        public async Task<IActionResult> Category(string language = "Vi")
        {
            List<CategoryNewsModel> r = await _cache.GetAsync<List<CategoryNewsModel>>($"CategoryNews_{language}");//IEnumerable
            if (r == null)
            {
                r = (from p in (await _Service.newsCategoriesServices.GetAllAsync())
                     select new CategoryNewsModel()
                     {
                         Id = p.Id,
                         Img = p.Img,
                         Name = p.Name,
                         ParentId = p.ParentId
                     }).ToList();
                await _cache.SetAsync<List<CategoryNewsModel>>($"CategoryNews_{language}", r);
            }
            _logger.WriteLog(_configuration, $"Category {language}:", $"Category {language}");
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
        [Route("[action]/{categoryId}")]
        public async Task<IActionResult> List(long categoryId, int? page, int? pageSize)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            long _Id = categoryId;
            int _Page = (page.HasValue ? page.Value : 1);
            int _PageSize = pageSize.HasValue ? pageSize.Value : 10;
            Func<Article, object> sqlOrder = s => s.Id;
            Expression<Func<Article, bool>> sqlWhere = u => (u.CategoryMain == _Id || _Id == 0);
            Expression<Func<Article, bool>> sqlWhereNew = u => (u.IsNews);
            var r = await _Service.articleServices.GetListAsync(sqlWhere, sqlOrder, true, _Page, _PageSize);
            _logger.WriteLog(_configuration, $"ListNews {categoryId}/{page}/{pageSize}", $"ListNews {categoryId}/{page}/{pageSize}");
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
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
        [Route("[action]/{Id}")]
        public async Task<IActionResult> Details(long Id)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            Expression<Func<Article, bool>> sqlWhereNew = u => (u.IsNews);
            var article = await _Service.articleServices.GetByIdAsync(Id);
            Expression<Func<Article, bool>> sqlWhere = u => (u.CategoryMain == article.CategoryMain && u.Id < article.Id);
            var a = new NewsDetails()
            {
                article = article,
                articles = null,// await _Service.articleServices.GetTopAsync(sqlWhereNew, 7),
                articleRelated = null//await _Service.articleServices.GetTopAsync(sqlWhere, 10)
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
