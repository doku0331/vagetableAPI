using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using vagetableAPI.ViewModels;

namespace vagetableAPI.Filters
{
    public class ExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);
            
            var result = new ResultViewModel
            {
                success = false,
                msg = actionExecutedContext.Exception.Message
            };

            actionExecutedContext.Response = 
                actionExecutedContext.Request.CreateResponse(result);
        }
    }
}