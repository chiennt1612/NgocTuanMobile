using Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IAllService _Service;

        public OrderController(
            ILogger<OrderController> logger,
            IAllService Service)
        {
            this._logger = logger;
            this._Service = Service;
            _logger.WriteLog("Starting Order page");
        }

        [HttpGet]
        [Route("[action]/{Id}")]
        public async Task<IActionResult> Detail(long Id)
        {
            var a = await _Service.contactServices.GetByIdAsync(Id);
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
