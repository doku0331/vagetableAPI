using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vagetableAPI.ViewModels
{
    public class ExpiredFoodViewModel
    {
        public int FoodId { get; set; }
        public int FridgeId { get; set; }
        public string fName { get; set; }
        public System.DateTime expire_date { get; set; }
        public string food_name { get; set; }
    }
}