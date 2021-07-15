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
using vagetableAPI.Service;
using System.IO;
using System.Web;

namespace vagetableAPI.Controllers
{
    [JwtAuthorize]
    public class FoodController : ApiController
    { 
        //建立存取資料庫的實體
        dbVagetableBasketEntities db = new dbVagetableBasketEntities();
        FridgeService fridgeService = new FridgeService();

        #region 取得食物

        /// <summary>
        ///  取得指定冰箱中的食物
        /// </summary>
        /// <param name="fridgeId"></param>
        /// <returns></returns>
        // GET: api/Food
        [HttpGet]
        [Route("api/food/Getfood/{fridgeid}")]
        public FoodListViewModel Getfood(int fridgeId)
        {
            if (!fridgeService.OwnFridgeCheck(fridgeId,User.Identity.Name)) {
                throw new CustomException("你沒有冰箱權限");
            }

            //取出冰箱中的食物
            var food = db.Food.Where(m => m.fridge_id == fridgeId).ToList();
            //把資料放入viewModel
            var model = new FoodListViewModel
            {
                //當前冰箱的id
                fridgeId = fridgeId,
                //冰箱的名稱
                FridgeName = db.Fridge.Where(x => x.fId == fridgeId).First().fName,
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

        /// <summary>
        /// 搜尋指定冰箱的食物
        /// </summary>
        /// <param name="fridgeId">指定冰箱id</param>
        /// <param name="search">搜尋字串</param>
        /// <returns></returns>
        [ResponseType(typeof(FoodListViewModel))]
        [HttpGet]
        [Route("api/food/search/{fridgeId}/")]
        public object Search(int fridgeId,string search)
        {
            //搜尋字串不為空白則搜尋
            if (string.IsNullOrWhiteSpace(search))
            {
                throw new CustomException("搜尋字串為空");
            }

            if (!fridgeService.OwnFridgeCheck(fridgeId, User.Identity.Name))
            {
                throw new CustomException("你沒有冰箱權限");
            }
           
            //撈出所有在指定冰箱中的食物
            var query = db.Food.Where(x => x.fridge_id == fridgeId).AsQueryable();
            
            //撈出符合搜尋字串的東西
            query = query.Where(x => x.food_name.Contains(search));
            
            //將搜尋到的資料按照日期排列
            query = query.OrderBy(x => x.expire_date);

            //把資料放入viewModel
            var result = new FoodListViewModel
            {
                fridgeId = fridgeId,
                FridgeName = db.Fridge.Where(x => x.fId == fridgeId).First().fName,
                food = query.ToList()
            };

            return result;

        }
        #endregion 

        /// <summary>
        /// 新增食物進冰箱
        /// </summary>
        /// <param name="fridgeId"></param>
        /// <param name="model">需要必要(食物名稱、類型、過期日)非必要(照片、價格、註解)</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/food/Create/{fridgeId}")]
        public object Create(int fridgeId, Food model)
        {
            if (!fridgeService.OwnFridgeCheck(fridgeId, User.Identity.Name))
            {
                throw new CustomException("你沒有冰箱權限");
            }

            //取得當前的 request 物件
            var httpRequest = HttpContext.Current.Request;
            //request 如有夾帶檔案
            if (httpRequest.Files.Count > 0)
            {
                //逐一取得檔案名稱
                foreach (string fileName in httpRequest.Files.Keys)
                {
                    //以檔案名稱從 request 的 Files 集合取得檔案內容
                    var file = httpRequest.Files[fileName];
                    //其他檔案處理
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            /*
            //檢查image有沒有東西
            if (model.ItemImage != null)
            {
                //取得檔名
                string filename = Path.GetFileName(model.ItemImage.FileName);
                //將檔案和伺服器上路徑合併
                string Url = Path.Combine(HttpContext.Current.Server.MapPath("~/Upload/"), filename);
                //將檔案儲存於伺服器上
                model.ItemImage.SaveAs(Url);
                //設定路徑
                model.NewFood.photo = filename;
            }
            
            //檢查模型有沒有通過驗證
            if (ModelState.IsValid == false)
            {
                throw new CustomException("輸入資料不符合規範");
            }
            model.NewFood.fridge_id = fridgeId;
            try
            {
                //將食物新增到資料表
                db.Food.Add(model.NewFood);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.ToString());
            }
            */
            return Ok("新增食物成功");

        }
    }
}