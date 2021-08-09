using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using vagetableAPI.Filters;
using vagetableAPI.Models;
using vagetableAPI.ViewModels;
using vagetableAPI.Service;

namespace vagetableAPI.Controllers
{
    [JwtAuthorize]
    public class fridgeController : ApiController
    {
        //建立存取資料庫的實體
        dbVagetableBasketEntities db = new dbVagetableBasketEntities();

        FridgeService fridgeService = new FridgeService();
        MembersDBService membersDBService = new MembersDBService();

        /// <summary>
        /// 列出所有冰箱
        /// </summary>
        /// <returns>冰箱物件</returns>
        [HttpGet]
        [Route("api/fridge/List")]
        public IEnumerable<Fridge> List()
        {
            //撈出使用者擁有的所有冰箱冰箱
            var fridges = (from m in db.Own_Fridge
                           join n in db.Fridge on m.fid equals n.fId
                           where m.account == User.Identity.Name
                           select n).ToList();
            return fridges;
        }


        /// <summary>
        /// 建立冰箱
        /// </summary>
        /// <param name="fridge"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/fridge/Create")]
        public object Create(FridgeCreateViewModel fridge)
        {
            //驗證模型是否通過
            if (!ModelState.IsValid)
            {
                throw new CustomException("冰箱建立失敗");
            }
            //建立list處存使用者清單
            List<Own_Fridge> ownerList = new List<Own_Fridge>();
            foreach (var Owner in fridge.Owners)
            {
                if (membersDBService.CheckExist(Owner))
                {
                    ownerList.Add(new Own_Fridge { account = Owner });
                }
                else
                {
                    throw new CustomException(Owner + "為不存在之會員");
                }
            }

            //把當下使用者也加入
            ownerList.Add(new Own_Fridge{ account = User.Identity.Name});

            //實體化新的冰箱模型
             Fridge newFridge = new Fridge
            {
                fName = fridge.fName
            };
            try
            {
                //新增
                db.Fridge.Add(newFridge);
                //新增時候上面的fridge的id會自動填入下面的 我也不知道為啥
                db.Own_Fridge.AddRange(ownerList);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.ToString());
            }

            //重導向回冰箱列表
            return Ok("冰箱建立成功");

        }
        /// <summary>
        /// 取得指定冰箱的使用者
        /// </summary>
        /// <param name="fridgeid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/fridge/GetOwner/{fridgeid}/")]
        public object GetOwner(int fridgeid)
        {
            if (!fridgeService.OwnFridgeCheck(fridgeid, User.Identity.Name))
            {
                throw new CustomException("你沒有冰箱權限");
            }
           
            var Owners = db.Own_Fridge.Where(n => n.fid == fridgeid).Select(x=> x.account).ToList();
            return Owners;
        }

        // TODO: 把新增成員做成連結讓人加入

        /// <summary>
        /// 修改指定冰箱的名稱或是成員
        /// </summary>
        /// <param name="fridgeid">冰箱id</param>
        /// <param name="frigEdit">冰箱編輯的資料</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/fridge/Edit/{fridgeid}/")]
        public object Edit(int fridgeid, FrigEditViewModel frigEdit)
        {
            if (!fridgeService.OwnFridgeCheck(fridgeid, User.Identity.Name))
            {
                throw new CustomException("你沒有冰箱權限");
            }
            //撈出當下冰箱資料
            var nowFridge = db.Fridge.Where(m => m.fId == fridgeid).FirstOrDefault();

            //確認是否有更改冰箱名稱
            if (!string.IsNullOrWhiteSpace(frigEdit.fName) && frigEdit.fName != nowFridge.fName)
            {
                nowFridge.fName = frigEdit.fName;
                db.SaveChanges();
            }
            
            //新增使用者不為空
            if (!string.IsNullOrWhiteSpace(frigEdit.user))
            {   
                //撈出會員
                var member = db.Member.Where(m => m.account == frigEdit.user).FirstOrDefault();
                //確認輸入的使用者是否已經是該冰箱擁有者
                var owner = db.Own_Fridge.Where(x => x.fid == fridgeid && x.account == frigEdit.user).FirstOrDefault();
                //使用者存在 且不存在當下使用者清單 則加入使用者為共用
                if (member == null)
                {
                    throw new CustomException("欲新增之會員不存在");
                }
                if (owner != null)
                {
                    throw new CustomException("會員已經是冰箱使用者");
                }
                try
                { 
                    //建立擁有者的物件
                    Own_Fridge owntemp = new Own_Fridge
                    {
                        fid = fridgeid,
                        account = frigEdit.user
                    };
                    //新增
                    db.Own_Fridge.Add(owntemp);
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    throw new CustomException(ex.ToString());
                }
            }
            //可能有編輯過名稱或成員，但兩者皆空也有可能
            return Ok("編輯完成");
        }

        /// <summary>
        /// 刪除冰箱
        /// </summary>
        /// <param name="fridgeid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/fridge/Delete/{fridgeid}")]
        public object Delete(int fridgeid)
        {
            if (!fridgeService.OwnFridgeCheck(fridgeid, User.Identity.Name))
            {
                throw new CustomException("你沒有冰箱權限");
            }
            try
            {
                //撈出該冰箱
                var fridge = (from m in db.Fridge
                              where m.fId == fridgeid
                              select m).FirstOrDefault();

                //撈出該冰箱相關之擁有者與食物全部刪除
                var del1 = db.Own_Fridge.Where(m => m.fid == fridgeid);
                var del2 = db.Food.Where(m => m.fridge_id == fridgeid);
                db.Own_Fridge.RemoveRange(del1);
                db.Food.RemoveRange(del2);
                //刪除冰箱
                db.Fridge.Remove(fridge);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new CustomException("刪除錯誤" + ex.ToString());
            }

            return Ok("刪除成功");
            
            
        }

        /// <summary>
        ///刪除冰箱的指定擁有者
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/fridge/OwnerDelete/{fid}/{userid}")]
        public object OwnerDel(int fid, string userid)
        {
            if (!fridgeService.OwnFridgeCheck(fid, User.Identity.Name))
            {
                throw new CustomException("你沒有冰箱權限");
            }
            //撈出該擁有者
            var owner = db.Own_Fridge.Where(x => x.account == userid && x.fid == fid).FirstOrDefault();
            try
            {
                //移除
                db.Own_Fridge.Remove(owner);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new CustomException("刪除錯誤" + ex.ToString());
            }

            return Ok("刪除成功");
            
        }
        public class AllFridgeDataViewModel
        {
            public string fname { get; set; }
            public IEnumerable<string> members { get; set; }
            public IEnumerable<Food> food { get; set; }
        }

        /// <summary>
        /// 列出所有擁有的冰箱和每個冰箱的使用者與食物
        /// </summary>
        /// <returns>(冰箱名，使用者們，冰箱內的食物)的集合</returns>
        [HttpGet]
        [Route("api/fridge/GetAllFridgeData")]
        public IEnumerable<AllFridgeDataViewModel> GetAllFridgeData()
        {
            //建立列表儲存食物
            List<AllFridgeDataViewModel> allFridgeData = new List<AllFridgeDataViewModel>();

            //撈出使用者擁有的所有冰箱冰箱
            var fridges = (from m in db.Own_Fridge
                           join n in db.Fridge on m.fid equals n.fId
                           where m.account == User.Identity.Name
                           select n).ToList();

            //歷遍所有冰箱撈出名稱成員與食物
            foreach (var fridge in fridges)
            {
                AllFridgeDataViewModel data = new AllFridgeDataViewModel();
                data.fname = fridge.fName;
                data.members = db.Own_Fridge.Where(n => n.fid == fridge.fId).Select(x => x.account).ToList();
                data.food = db.Food.Where(m => m.fridge_id == fridge.fId).OrderBy(x => x.expire_date).ToList();

                allFridgeData.Add(data);
            }

            return allFridgeData;
        }
    }
}