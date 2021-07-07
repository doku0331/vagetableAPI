using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using vagetableAPI.Models;
namespace vagetableAPI.ViewModels
{
    public class FoodListViewModel
    {
        public int fridgeId { get; set; }
        public string FridgeName { get; set; }
        public IEnumerable<Food> food { get; set; }
    }
}