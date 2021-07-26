using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vagetableAPI.ViewModels
{
    public class item
    {
        public string name { get; set; }
        public string amount { get; set; }
    }

    public class ingridentList
    {
        public List<item> ingredient { get; set; }
    }
}