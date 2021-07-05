using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Jose;
using System.Web.Configuration;
using System.Text;
using System.Web;
using vagetableAPI.Security;
using vagetableAPI.Models;
using vagetableAPI.ViewModels;

namespace vagetableAPI.Controllers
{
    public class TokenController : ApiController
    {
        [Route("api/token")]
        // POST api/values
        public object Post(MembersLoginViewModel loginData)
        {
            
            //從config取得密鑰
            var secret = WebConfigurationManager.AppSettings["SecretKey"].ToString();

            // TODO: 真實世界檢查帳號密碼
            if (loginData.Account == "wellwind" && loginData.Password == "1234")
            {
                //製作payload
                var payload = new JwtObject()
                {
                    Account = loginData.Account,
                    Expire = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"])).ToString()
                };

                //返回token
                return new
                {
                    Result = true,
                    token = Jose.JWT.Encode(payload, Encoding.UTF8.GetBytes(secret), JwsAlgorithm.HS256)
                };
            }
            else
            {
                //返回錯誤的token
                return new
                {
                    status = false,
                    token = "Account Or Password Error"
                };
            }
        
        }

    }
}
