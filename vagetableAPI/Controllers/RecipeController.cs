using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using vagetableAPI.Filters;
using vagetableAPI.Models;
using vagetableAPI.ViewModels;

namespace vagetableAPI.Controllers
{
    [RoutePrefix("api/recipe")]
    public class RecipeController : ApiController
    {

        //建立存取資料庫的實體
        dbVagetableBasketEntities db = new dbVagetableBasketEntities();

        /// <summary>
        /// 取得每頁六筆的食譜資料
        /// </summary>
        /// <param name="page">頁數</param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/")]
        public List<Recipe> List(int page = 1)
        {
            //設定每一頁有多少資料
            var pageSize = 6;
            //取得該頁數的資料
            var result = db.Recipe.OrderBy(x => x.created_time).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }

        //TODO: 同食物 確認多人同時上傳時是否會正確回傳剛才新增的ID 如果不行就要取消自動增加 在新增前就取得id 請確認是否有可能有資料庫撞號碼
        /// <summary>
        /// 新增食譜 返回新增的食譜的id， 注意:請使用 Content-Type:multipart/form-data 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthorize]
        [Route("Create/")]
        public object Create()
        {
            // 如果 Content-Type 沒有 multipart/form-data 就回傳 415
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            //取得httpcontext中的request
            var request = HttpContext.Current.Request;
            //取得請求中的表單資浪
            var formData = request.Form;
            //建立新的食譜
            var newRecipe = new Recipe
            {
                account = User.Identity.Name,
                recipe_name = formData["recipe_name"],
                step = formData["step"],
                created_time = DateTime.Now
            };

            //若有夾帶圖片
            if (request.Files.Count > 0 && request.Files[0].ContentType != null)
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
                newRecipe.recipe_photo = file.FileName;
            }
            //將食物新增到資料表
            try
            {
                db.Recipe.Add(newRecipe);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            var recipeId = db.Recipe.Where(x => x.recipe_name == newRecipe.recipe_name)
                           .Select(x => x.recipe_id).Max();

            return new { RecipeId =  recipeId };
        }

        //TODO: 測試此api
        /// <summary>
        /// 新增食材
        /// </summary>
        /// <param name="recipeId">指定食譜</param>
        /// <param name="model">食材列表</param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateIngrident/{recipeId}")]
        public object CreateIngrident(int recipeId, ingridentList model)
        {
            List<Ingredient> newIngridentList = new List<Ingredient>();
            foreach (var ingrident in model.ingredient)
            {
                newIngridentList.Add(
                    new Ingredient 
                    {
                        recipe_id =recipeId ,
                        name = ingrident.name ,
                        amount = ingrident.amount 
                    });
            }
            //將興趣存入資料表
            try
            {
                db.Ingredient.AddRange(newIngridentList);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.ToString());
            }
            return Ok("新增食材成功");
        }
    }
}
