//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace vagetableAPI.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public partial class Own_Fridge
    {
        public int id { get; set; }
        public int fid { get; set; }
        public string account { get; set; }


        [JsonIgnore]
        public virtual Fridge Fridge { get; set; }

        [JsonIgnore]
        public virtual Member Member { get; set; }
    }
}
