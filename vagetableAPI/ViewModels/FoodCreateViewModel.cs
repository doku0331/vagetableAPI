using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using vagetableAPI.Models;
using System.Web;

namespace vagetableAPI.ViewModels
{
    public class FoodCreateViewModel
    {
        //新商品的圖片
        [DisplayName("食物圖片")]
        public HttpPostedFileBase ItemImage { get; set; }

        //新商品本體
        public Food NewFood { get; set; }
    }
}