using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using vagetableAPI.Models;
using vagetableAPI.Service;
using vagetableAPI.Filters;
using vagetableAPI.ViewModels;

namespace vagetableAPI.Controllers
{
    [JwtAuthorize]
    public class MemberController : ApiController
    {
        //建立存取資料庫的實體
        dbVagetableBasketEntities db = new dbVagetableBasketEntities();

        // 宣告Members資料表的Service物件
        private readonly MembersDBService membersService = new MembersDBService();

        // 宣告寄信用的Service物件
        private readonly MailService mailService = new MailService();

        // 宣告jwt用的Service物件
        private readonly JwtService jwtService = new JwtService();

        /// <summary>
        /// 登入並回傳token
        /// </summary>
        /// <param name="loginData">帳號密碼</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("api/Login")]
        public Object Login(MembersLoginViewModel loginData)
        {
            // 依帳密取得會員並指定給member
            var member = db.Member
                .Where(m => m.account == loginData.Account)
                .FirstOrDefault();

            //若member為null，表示會員未註冊
            if (member == null)
            {
                throw new CustomException("帳號或密碼錯誤，登入失敗！");
            }
            else if (membersService.PasswordCheck(member, loginData.Password) && member.authCode.First() == ' ')
            {
                var token = jwtService.GenerateToken(loginData);
                var intrest = db.Interest.Where(x => x.account == loginData.Account).Select(x=> x.tag).ToList();
                return new { Token =  token , Intrest = intrest};
            }
            else if (!membersService.PasswordCheck(member, loginData.Password))
            {
                throw new CustomException("密碼錯誤，登入失敗！");
            }
            else
            {
                throw new CustomException("您尚未進行會員驗證，請先至信箱收取驗證信！");
            }

        }

        public class RegisterViewModel
        {
            public Member newMember { get; set; }
            public List<string> interest { get; set; }
        }
        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="model">會員的資料，帳號、密碼、姓名、email</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("api/Register")]
        public object Register(RegisterViewModel model)
        {
            //若模型沒有通過驗證則顯示目前的View
            if (ModelState.IsValid == false)
            {
                throw new CustomException("未通過模型驗證，不合法資料");
            }

            // 依帳號取得會員並指定給member
            var member = db.Member
                .Where(m => m.account == model.newMember.account)
                .FirstOrDefault();

            //若member為null，表示會員未註冊
            if (member != null)
            {
                throw new CustomException("此帳號己有人使用，註冊失敗！");
            }
            //發驗證信並新增會員
            try
            {
                // Hash 使用者的密碼
                model.newMember.password = membersService.HashPassword(model.newMember.password);

                // 先給一組驗證碼，並放入模型
                model.newMember.authCode = mailService.GetValidateCode();

                // 寄驗證信給新的會員
                mailService.SendRegisterMail(mailService.GetRegisterMailBody(model.newMember.account, model.newMember.authCode), model.newMember.email);

                //將會員記錄新增到Member資料表
                db.Member.Add(model.newMember);

                db.SaveChanges();

            }
            catch (Exception e)
            {
                throw new CustomException(e.ToString());
            }
            //建立list處存興趣
            List<Interest> intrestsList = new List<Interest>();
            foreach (var interest in model.interest)
            {
                intrestsList.Add(new Interest { account = model.newMember.account, tag = interest });
            }
            //將興趣存入資料表
            try
            {
                db.Interest.AddRange(intrestsList);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.ToString());
            }

            return Ok("註冊成功");

        }

        public class RegisterValidateViewModel
        {
            public string Account { get; set; }
            public string authCode { get; set; }
        }
        /// <summary>
        /// 認證會員
        /// </summary>
        /// <param name="auth">帳號跟驗證碼</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("api/RegisterValidate")]
        public object RegisterValidate(RegisterValidateViewModel auth)
        {
            try
            {
                var member = db.Member
                           .Where(m => m.account == auth.Account)
                           .FirstOrDefault();

                if (member == null)
                {
                    throw new CustomException("帳號輸入錯誤！");
                }
                else if (member.authCode.First() == ' ')
                {
                    throw new CustomException("已經通過驗證，直接登入即可");
                }
                else if (member.authCode == auth.authCode)
                {
                    member.authCode = string.Empty;
                    db.SaveChanges();
                    return Ok("驗證成功，請去登入");
                }
                else
                {
                    throw new CustomException("驗證碼輸入錯誤！");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


    }
}

/* 等我確認好基礎功能再實作
 * 
// Get：Member/ForgetPassword
public ActionResult ForgetPassword()

{
    return View();
}

// Post：Member/ForgetPassword
[HttpPost]
public ActionResult ForgetPassword(string account, string email)
{

    try
    {
        var member = db.Member
                   .Where(m => m.account == account)
                   .FirstOrDefault();

        if (member != null && member.authCode.First() == ' ')
        {
            if (member.email == email)
            {
                // 先給一組驗證碼，並存入資料庫的密碼
                member.password = mailService.GetValidateCode();

                // 寄驗證信給新的會員
                mailService.SendRegisterMail(mailService.GetRegisterMailBody(member.account, member.password), member.email);

                // Hash 此密碼
                member.password = membersService.HashPassword(member.password);

                // Linq 更新語法
                (from m in db.Member where m.account == account select m)
                .Single().password = member.password;

                // 儲存密碼到資料庫

                db.SaveChanges();

                ViewBag.Message = "請至信箱收取驗證碼，以此驗證碼當作密碼登入，登入後請先變更密碼";
            }
        }
        else
        {
            ViewBag.Message = "Email輸入錯誤，或是您尚未進行會員驗證！";
        }
    }

    catch (Exception ex)
    {

    }
    return View();
}
[HttpPost]
// POST：Member/ModifyPassword
public ActionResult ModifyPassword(string account, string password, string newpassword)
{
    var member = db.Member
                   .Where(m => m.account == account)
                   .FirstOrDefault();

    if (member != null)
    {
        try
        {
            if (member.authCode.First() != ' ')
            {
                ViewBag.Message = "您尚未進行會員驗證，請先至信箱收取驗證信，驗證成功後再來修改密碼";
            }
            else
            {
                ViewBag.Message = membersService.ChangePassword(member.account, password, newpassword);

                if (ViewBag.Message == "密碼變更成功")
                {
                    return RedirectToAction("Login");
                }
            }

        }

        catch (Exception ex)
        {

        }
    }

    return View();

}

[Authorize]
// GET：Member/LoginModifyPassword
public ActionResult LoginModifyPassword()
{
    return View();
}
[Authorize]
[HttpPost]
// POST：Member/LoginModifyPassword
public ActionResult LoginModifyPassword(string password, string newpassword)
{
    string account = User.Identity.Name;

    var member = db.Member
                   .Where(m => m.account == account)
                   .FirstOrDefault();

    if (member != null)
    {
        try
        {
            ViewBag.Message = membersService.ChangePassword(member.account, password, newpassword);
            if (ViewBag.Message == "密碼變更成功")
            {
                return RedirectToAction("Logout");
            }

        }

        catch (Exception ex)
        {

        }
    }
    return View();
}

*/