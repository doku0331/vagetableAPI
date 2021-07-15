using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace vagetableAPI.ViewModels
{
    public class RegisterViewModel
    {
        [DisplayName("帳號")]
        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "帳號長度請介於6到15之間")]
        public string account { get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        public string password { get; set; }

        [DisplayName("姓名")]
        [StringLength(10, ErrorMessage = "最多十字元")]
        public string mName { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "請輸入Email")]
        [StringLength(100, ErrorMessage = "Email長度最多100字元")]
        [EmailAddress(ErrorMessage = "這不是Email格式")]
        public string email { get; set; }

    }
}