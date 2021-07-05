using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using vagetableAPI.Security;
using System.Web.Configuration;

using System.Security.Claims;
using System.Security.Principal;
using System.Threading;

namespace vagetableAPI.Filters
{
    public class JwtAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
           //從config取得密鑰
            var secret = WebConfigurationManager.AppSettings["SecretKey"].ToString();

            if (actionContext.Request.Headers.Authorization == null
                || actionContext.Request.Headers.Authorization.Scheme != "Bearer")
            {
                //回覆401
                setErrorResponse(actionContext, "驗證錯誤");
            }
            else
            {
                try
                {
                    //解開header的jwt
                    var jwtObject = Jose.JWT.Decode<JwtObject>(
                        actionContext.Request.Headers.Authorization.Parameter,
                        Encoding.UTF8.GetBytes(secret),
                        JwsAlgorithm.HS256);

                    if (IsTokenExpired(jwtObject.Expire.ToString()))
                    {
                        setErrorResponse(actionContext, "Token Expired");
                    }
                   //把拆解下來的jwt填入claim
                    Claim[] claims = new Claim[]
                    {
                    new Claim(ClaimTypes.Name,jwtObject.Account),
                    new Claim(ClaimTypes.NameIdentifier,jwtObject.Account)
                    };

                    //製作新的claimsIdentity
                    var claimsIdentity = new ClaimsIdentity(claims, "NAME");
                    //設定principal 因為本專案無腳色所以null
                    var principal = new GenericPrincipal(claimsIdentity, null);
                    //把principal套用到Thread與httpoontext上
                    Thread.CurrentPrincipal = principal;
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.User = principal;
                    }

                }
                catch (Exception ex)
                {
                    setErrorResponse(actionContext, ex.Message);
                }
            }
            base.OnActionExecuting(actionContext);
        }

        //回覆401
        private static void setErrorResponse(HttpActionContext actionContext, string message)
        {
            var response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, message);
            actionContext.Response = response;
        }

        //驗證token時效
        public bool IsTokenExpired(string dateTime)
        {
            return Convert.ToDateTime(dateTime) < DateTime.Now;
        }
    }
}