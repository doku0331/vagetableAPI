using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using vagetableAPI.ViewModels;

namespace vagetableAPI.Filters
{
    public class IgnoreResultAttribute : Attribute
    {
    }
    public class ResultAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            //有exception不進入
            if (actionExecutedContext.Exception != null)
            {
                return;
            }

            //查看是否有掛ignoreResult
            //有則跳過
            var ignoreResult1 = actionExecutedContext.ActionContext.ActionDescriptor.GetCustomAttributes<IgnoreResultAttribute>().FirstOrDefault();
            var ignoreResult2 = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<IgnoreResultAttribute>().FirstOrDefault();
            
            if (ignoreResult1 != null || ignoreResult2 != null)
            {
                return;
            }

            //取出內容物 變成object
            var objectContent = actionExecutedContext.Response.Content as ObjectContent;
            
            //若objectContent不為null則填入value
            var data = objectContent?.Value;

            var result = new ResultViewModel
            {
                success = true,
                data = data
            };

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(result);
        }
    }
}