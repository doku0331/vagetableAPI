using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//驗證模型的函式庫
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
// 登入用 ViewModel

namespace vagetableAPI.ViewModels
{
    public class MembersLoginViewModel
    {

        [DisplayName("會員帳號")]
        [Required(ErrorMessage = "請輸入會員帳號")]
        public string Account { get; set; }

        [DisplayName("會員密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        public string Password { get; set; }
    }
}