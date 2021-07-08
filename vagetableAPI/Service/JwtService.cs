using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using vagetableAPI.Security;
using vagetableAPI.ViewModels;

namespace vagetableAPI.Service
{
    public class JwtService
    {
        public string GenerateToken(MembersLoginViewModel loginData)
        {

            //從config取得密鑰
            var secret = WebConfigurationManager.AppSettings["SecretKey"].ToString();
          
            //製作payload
            var payload = new JwtObject()
            {
                Account = loginData.Account,
                Expire = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"])).ToString()
            };

            var token = Jose.JWT.Encode(payload, Encoding.UTF8.GetBytes(secret), JwsAlgorithm.HS256);
            return token;
           
        }
    }
}