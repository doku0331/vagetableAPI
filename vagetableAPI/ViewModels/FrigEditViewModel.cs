using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using vagetableAPI.Models;
namespace vagetableAPI.ViewModels
{
    public class FrigEditViewModel
    {
        public IEnumerable<Own_Fridge> Owner { get; set; }
        public string fName { get; set; }
        public string user { get; set; }
    }
}