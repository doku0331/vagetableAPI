using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace vagetableAPI.ViewModels
{
    public class FridgeCreateViewModel
    {

        [Required]
        [StringLength(10, ErrorMessage = "名稱不可大於10字元")]
        public string fName { get; set; }

        public List<String> Owners{ get; set; }
    }
}