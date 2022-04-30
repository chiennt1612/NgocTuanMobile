using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Utils
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ModelState.IsValid == false)
            {
                filterContext.Result = new JsonResult(
                    new Utils.Models.ResponseOK()
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = 0,
                        UserMessage = LanguageAll.Language.ModelStateInValid,
                        InternalMessage = LanguageAll.Language.ModelStateInValid,
                        MoreInfo = LanguageAll.Language.ModelStateInValid,
                        data = filterContext.ModelState.ToList()
                    })
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
        }
    }
}
