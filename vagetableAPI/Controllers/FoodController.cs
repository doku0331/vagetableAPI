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
    [JwtAuthorize]
    public class FoodController : ApiController
    { 
        //建立存取資料庫的實體
        dbVagetableBasketEntities db = new dbVagetableBasketEntities();

        /// <summary>
        /// 取得指定冰箱中的食物
        /// </summary>
        /// <param name="fridgeid">冰箱id</param>
        /// <returns></returns>
        /// 
        // GET: api/Food
        [HttpGet]
        [Route("api/food/Getfood/{fridgeid}")]
        public FoodListViewModel Getfood(int fridgeid)
        {
            //找出冰箱跟其使用者資料
            var OwnFridge = db.Own_Fridge.Where(x => x.fid == fridgeid).FirstOrDefault();

            //判斷使用者是否擁有該冰箱
            if (User.Identity.Name != OwnFridge.account)
            {
                throw new CustomException("你沒有冰箱權限");
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

        /// <summary>
        /// 取得過期食物
        /// </summary>
        /// <returns>返回食物名稱 該食物過期日 該食物所屬冰箱</returns>
        [HttpGet]
        [Route("api/food/expire")]
        public object expire()
        {
            //撈出使用者擁有的冰箱
            var fridge = (from m in db.Own_Fridge
                          join n in db.Fridge on m.fid equals n.fId
                          where m.account == User.Identity.Name
                          select n).ToList();

            //儲存過期食物的列表
            var list = new List<Food>();
            
            //歷遍冰箱找出過期食物放進list
            foreach (var item in fridge)
            {
                var allfood = db.Food.Where(x => x.fridge_id == item.fId).ToList();
                foreach (var food in allfood)
                {
                    if ((food.expire_date - DateTime.Now) < TimeSpan.FromDays(5))
                    {
                        list.Add(food);
                    }
                }
            }
            //過期食物加上冰箱儲存為expiredfoodviewmodel
            var expireds = (from x in list
                            join y in fridge on x.fridge_id equals y.fId
                            select new ExpiredFoodViewModel
                            {
                                food_name = x.food_name,
                                expire_date = x.expire_date,
                                fName = y.fName
                            }).ToList();

            return expireds;

        }
    }
}