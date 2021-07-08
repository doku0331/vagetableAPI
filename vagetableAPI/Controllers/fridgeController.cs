using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using vagetableAPI.Filters;
using vagetableAPI.Models;

namespace vagetableAPI.Controllers
{
    [JwtAuthorize]
    public class fridgeController : ApiController
    {
        //建立存取資料庫的實體
        dbVagetableBasketEntities db = new dbVagetableBasketEntities();

        [HttpGet]
        [Route("api/fridge/frigList")]
        public object FrigList()
        {
            //撈出使用者擁有的所有冰箱冰箱
            var fridges = (from m in db.Own_Fridge
                           join n in db.Fridge on m.fid equals n.fId
                           where m.account == User.Identity.Name
                           select n).ToList();

            return fridges;
        }



    }
}
