using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using vagetableAPI.Models;
using vagetableAPI.Filters;
using System.Web.Http.Description;
using vagetableAPI.ViewModels;


namespace vagetableAPI.Controllers
{
    [Result]
    [JwtAuthorize]
    public class FoodController : ApiController
    {
        dbVagetableBasketEntities db = new dbVagetableBasketEntities();

        /// <summary>
        /// 取得指定冰箱中的食物
        /// </summary>
        /// <param name="fridgeid">冰箱id</param>
        /// <returns></returns>
        /// 
        [ResponseType(typeof(FoodListViewModel))]
        // GET: api/Food
        [HttpGet]
        [Route("api/food/{fridgeid}")]
        public FoodListViewModel Get(int fridgeid)
        { 
            //找出冰箱跟其使用者資料
            var OwnFridge = db.Own_Fridge.Where(x => x.fid == fridgeid).FirstOrDefault();

            //判斷使用者是否擁有該冰箱
            if (User.Identity.Name != OwnFridge.account)
            {
            //    throw new CustomException("你沒有冰箱權限");
            }
            var food = db.Food.Where(m => m.fridge_id == fridgeid).ToList();
           
            //把資料放入viewModel
            var model = new FoodListViewModel
            {
               
                //當前冰箱的id
                fridgeId = fridgeid,
                //冰箱的名稱
                FridgeName = db.Fridge.Where(x => x.fId == fridgeid).First().fName,
                //冰箱內食物資料
                food = food,
            };
            return model;        
        }

    }
}
