using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Utils.Tokens;
using Utils.Tokens.Interfaces;

namespace StaffAPI.Helper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MyAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.RequestServices.GetService(typeof(ITokenCreationService)) as TokenCreationService;
            var encodedString = context.HttpContext.Request.Headers["Authorization"];
            var a = token.ValidateToken(encodedString);
            if (a == null || a == default)
            {
                context.Result = new JsonResult(
                    new Utils.Models.ResponseBase(
                        LanguageAll.Language.Unauthorized, LanguageAll.Language.Unauthorized,
                        LanguageAll.Language.Unauthorized, 0, 401))
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
        }
    }
}
