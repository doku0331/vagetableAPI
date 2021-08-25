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
        /// 取得指定冰箱中的食物
        /// </summary>
        /// <param name="fridgeId">指定冰箱</param>
        /// <param name="page">頁數</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/food/Getfood/{fridgeid}")]
        public FoodListViewModel Getfood(int fridgeId)
        {
            if (!fridgeService.OwnFridgeCheck(fridgeId,User.Identity.Name)) {
                throw new CustomException("你沒有冰箱權限");
            }

            //取出冰箱中的食物
            var food = db.Food.Where(m => m.fridge_id == fridgeId);
            //把資料放入viewModel
            var model = new FoodListViewModel
            {
                //當前冰箱的id
                fridgeId = fridgeId,
                //冰箱的名稱
                FridgeName = db.Fridge.Where(x => x.fId == fridgeId).First().fName,
                //冰箱內食物資料
                food = food.OrderBy(x => x.expire_date).ToList(),
            };
            return model;
        }

        /// <summary>
        /// 依照每五個為一頁取得指定冰箱的食物
        /// </summary>
        /// <param name="fridgeId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/food/GetFoodByPage/{fridgeid}")]
        public FoodListViewModel GetFoodByPage(int fridgeId, int page = 1)
        {
            if (!fridgeService.OwnFridgeCheck(fridgeId, User.Identity.Name))
            {
                throw new CustomException("你沒有冰箱權限");
            }

            //設定每一頁有多少資料
            var pageSize = 5;
            //取出冰箱中的食物
            var food = db.Food.Where(m => m.fridge_id == fridgeId);
            //把資料放入viewModel
            var model = new FoodListViewModel
            {
                //當前冰箱的id
                fridgeId = fridgeId,
                //冰箱的名稱
                FridgeName = db.Fridge.Where(x => x.fId == fridgeId).First().fName,
                //冰箱內食物資料
                food = food.OrderBy(x => x.expire_date).Skip((page - 1) * pageSize).Take(pageSize).ToList(),
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

        //TODO: 把上傳的圖片做重新命名的處理
        //TODO: 確認多人同時上傳同名食物時 因為自動增加 所以要先查詢一次 才能取得剛剛加入的食物ID 資料庫的處理為何
        /// <summary>
        /// 新增食物進冰箱 注意:請使用 Content-Type:multipart/form-data  需要必要(food_name、type、expire_date)非必要(photo、價格、註解)
        /// </summary>
        /// <param name="fridgeId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/food/Create/{fridgeId}")]
        public object Create(int fridgeId)
        {
            if (!fridgeService.OwnFridgeCheck(fridgeId, User.Identity.Name))
            {
                throw new CustomException("你沒有冰箱權限");
            }
            // 如果 Content-Type 沒有 multipart/form-data 就回傳 415
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            //取得httpcontext中的request
            var request = HttpContext.Current.Request;
            //取得請求中的表單資浪
            var formData = request.Form;
            //建立新的食物
            var newfood = new Food
            {
                fridge_id = fridgeId,
                food_name = formData["food_name"],
                type = formData["type"],
                expire_date = DateTime.Parse(formData["expire_date"]),
                comment = formData["comment"]
            };
            //建立log物件實體
            var log = new Log
            {
                account = User.Identity.Name,
                fridge_id = fridgeId,
                buy_time = DateTime.Now,
                type = formData["type"]
            };

            if (formData["price"]!=null)
            {
                int price = Int32.Parse(formData["price"]);
                newfood.price = price;
                log.price = price;
            }
            //若有夾帶圖片
            if (request.Files.Count > 0 && request.Files[0].ContentType!=null)
            {
                //取得圖片 預設只有一張
                var file = request.Files[0];
                //設定存檔目標路徑
                var savePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Upload/";
                //如果不存在就建立
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                //存檔
                file.SaveAs(savePath + file.FileName);
                //把檔案名塞進新資料
                newfood.photo = file.FileName;
            }
            //將食物新增到資料表
            try
            {
                db.Food.Add(newfood);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.ToString());
            }
            //把剛剛新增的食物的id放進紀錄
            log.food_id = db.Food.Where(x => x.food_name == newfood.food_name).Select(x => x.id).Max();
            try
            {
                //將log新增到log
                db.Log.Add(log);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.ToString());
            }
            return Ok("新增食物成功");
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="fridgeid">冰箱id</param>
        /// <param name="id">食物id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/food/Delete/{fridgeid}/{id}")]
        public object delete(int fridgeid, int id)
        {
            try
            {
                var deletefood = db.Food.Where(x => x.fridge_id == fridgeid && x.id == id).FirstOrDefault();
                db.Food.Remove(deletefood);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new CustomException(ex.ToString());
            }

            return Ok("完成刪除");
        }
    }
}