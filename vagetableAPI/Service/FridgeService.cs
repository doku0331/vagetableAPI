using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using vagetableAPI.Models;

namespace vagetableAPI.Service
{
    public class FridgeService
    {
        //建立存取資料庫的實體
        dbVagetableBasketEntities db = new dbVagetableBasketEntities();

        //確認使用者是否有冰箱權限
        public bool OwnFridgeCheck(int fridgeid, string name)
        {
            //找出冰箱跟其使用者資料
            var OwnFridge = db.Own_Fridge.Where(x => x.fid == fridgeid && x.account == name).FirstOrDefault();
             //判斷當下使用者是否擁有該冰箱
            if(OwnFridge!= null)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }
}