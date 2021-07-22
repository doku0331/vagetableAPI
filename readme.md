請自行在專案中加入以下檔案  
App_Data/dbVagetableBasket.mdf  
Services/setting.json

setting.json格式為  
{
  "gmail_account": "寄出驗證信的信箱帳號",  
  "gmail_password": "寄出驗證信的信箱密碼",  
  "gmail_mail": "寄出驗證信的信箱地址"  
}  

回傳格式統一為
```
{
  "success":是否成功,
  "msg":若失敗的訊息,
  "data":資料
}
```
所有的都必須在header加上這個使這次的請求回傳json  
Content-Type: application/json  
除了不須授權的其餘都要在header加上授權token  
Authorization: Bearer {token}   
如果有圖片
則需加入
Content-Type: multipart/form-data


* Food
* GET /api/food/Getfood/{fridgeId}
  * 取得指定冰箱中的食物

* GET /api/food/expire
  * 取得過期食物

* GET /api/food/search/{fridgeId}
  * 搜尋指定冰箱的食物

* POST /api/food/Create/{fridgeId}
  * 新增食物進冰箱 
  * 注意: 若有圖片請使用 Content-Type:multipart/form-data 
  * 必要參數(名稱food_name、類別type、過期日expire_date)
  * 可以不填(照片photo、價格price、附註comment)
  *  request 
  ```
  {
    food_name:string, 
    type:string, 
    expire_date:datetime, 
    photo:file, 
    price:int,
    comment:string,
  }
  ```
  * response 
  ```
  { 
    "success": true,
    "msg": null,
    "data": token
  }
  ```
  
* GET /api/food/Delete/{fridgeid}/{id}
  * 刪除指定冰箱中的指定食物

* fridge
* GET /api/fridge/List
  * 列出所有當下登入使用者的冰箱

* POST /api/fridge/Create
  * 建立冰箱

* GET /api/fridge/GetOwner/{fridgeid}
  * 取得指定冰箱的使用者

* POST /api/fridge/Edit/{fridgeid}
  * 修改指定冰箱的名稱或是成員

* GET /api/fridge/Delete/{fridgeid}
  * 刪除冰箱

* GET /api/fridge/OwnerDelete/{fid}/{userid}
  * 刪除冰箱的指定擁有者

* Member
* POST /api/Login
  * 登入並回傳token
  * request 
  ```{Account:string, Password:string}```
  * response 
  ```
  { 
    "success": true,
    "msg": null,
    "data": token
  }
  ```

* POST /api/Register
  * 註冊

* POST /api/RegisterValidate
  * 認證會員

RecipeShow/HideList OperationsExpand Operations
GET /api/recipe/List
取得每頁六筆的食譜資料

