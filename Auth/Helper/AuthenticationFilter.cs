using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Utils.Tokens;
using Utils.Tokens.Interfaces;

namespace Auth.Helper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MyAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.RequestServices.GetService(typeof(ITokenCreationService)) as TokenCreationService;
            var encodedString = context.HttpContext.Request.Headers["Authorization"];
            if (!token.ValidateToken(encodedString))
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
