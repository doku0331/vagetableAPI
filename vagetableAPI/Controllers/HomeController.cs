using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using vagetableAPI.Filters;

namespace vagetableAPI.Controllers
{
    public class HomeController : ApiController
    {
        [JwtAuthorize]
        [Route("api/home")]
        // GET: api/
        public IEnumerable<string> Get()
        {
            if (User.Identity.IsAuthenticated)
            {
                return new string[] { User.Identity.Name, "value2" };

            }
            return new string[] {"B", "value2" };
        }
    }
}
