using Auth.Models;
using Auth.Services.Interfaces;
using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IAllService _Service;
        private readonly IProfile _profile;

        public OrderController(
            ILogger<OrderController> logger,
            IAllService Service,
            IProfile profile)
        {
            this._logger = logger;
            this._Service = Service;
            this._profile = profile;
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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> List(int? page, int? pageSize, [FromBody] OrderSearchModel o)
        {
            int _Page = (page.HasValue ? page.Value : 1);
            int _PageSize = pageSize.HasValue ? pageSize.Value : 10;
            Func<Contact, object> sqlOrder = s => s.Id;
            Expression<Func<Contact, bool>> sqlWhere = u => (
                    u.PaymentMethod == 3 && u.UserId == long.Parse(User.Claims.GetClaimValue(ClaimTypes.NameIdentifier))
                    && (!o.FromDate.HasValue || o.FromDate.HasValue && u.ContactDate >= o.FromDate.Value)
                    && (!o.ToDate.HasValue || o.ToDate.HasValue && u.ContactDate <= o.ToDate.Value)
                    && (!o.StatusId.HasValue || o.StatusId.HasValue && u.StatusId == o.StatusId.Value)
                    && (o.IsService && u.ServiceId.HasValue && !o.IsService && !u.ServiceId.HasValue)
                    );
            var r = await _Service.contactServices.GetListAsync(sqlWhere, sqlOrder, true, _Page, _PageSize);
            var r1 = new BaseEntityList<OrderModel>()
            {
                page = _Page,
                pageSize = _PageSize,
                totalRecords = r.totalRecords
            };
            r1.list = from b in r.list
                      select new OrderModel()
                      {
                          Id = b.Id,
                          OrderDate = b.ContactDate.Value,
                          StatusId = b.StatusId.Value,
                          FeeShip = 0,
                          Total = b.Price,
                          Description = b.Description,
                          Address = b.Address,
                          Email = b.Email,
                          Fullname = b.Fullname,
                          Mobile = b.Mobile,
                          PaymentMethod = 3,
                          ServiceId = b.ServiceId
                      };
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = r1
            });
        }
    }
}
/*
  "data": {
    "isCompany": true, // False hồ sơ cá nhân; True hồ sơ doanh nghiệp/ tổ chức
    "companyName": "string",
    "phoneNumber": "string",
    "email": "user@example.com",
    "address": "string",
    "fullname": "string",
    "birthday": null,
    "personID": "string",
    "issueDate": "2022-06-30T07:56:08",
    "issuePlace": "string",
    "waterCompany": null,
    "customerCodeList": [
      "0.012-04083" // Id nhà máy nước; Mã hợp đồng (Sau khi đăng ký hợp đồng). Thông tin dùng truy xuất hóa đơn tự động hàng tháng
    ]
  },
 */