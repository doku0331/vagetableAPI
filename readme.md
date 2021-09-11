## 更動紀錄
* 2021/8/30 修正冰箱的新增功能輸入Null會出問題的bug
* 2021/8/25 
  * 把過期食物的api修改成回傳回冰箱與食物的id
  * 修改依照冰箱取得食物的api改為可以依照頁數取得與取得全部
* 2021/8/9 新增GetAllFridgeData 的API於fridge類別中
  
## 注意事項
請自行在專案中加入以下檔案  
App_Data/dbVagetableBasket.mdf  
檔案位置:
https://drive.google.com/file/d/1dnC_vy25c9q-NrIhDbTyapIN2YDQp7Uk/view?usp=sharing
Services/setting.json

setting.json格式為  
```
{
  "gmail_account": "寄出驗證信的信箱帳號",  
  "gmail_password": "寄出驗證信的信箱密碼",  
  "gmail_mail": "寄出驗證信的信箱地址"  
}  
```

回傳格式統一為
```
{
  "success":是否成功,
  "msg":若失敗的訊息,
  "data":資料
}
```
所有的都必須在header加上這個  
Content-Type: application/json
如果有需要傳送圖片要加上  
Content-Type:multipart/form-data 
如果需要授權則要加上  
Authorization: Bearer {token}   
如果有圖片
則需加入
Content-Type: multipart/form-data


## member 關於會員類的api
* POST /api/Login
  * 登入並回傳token
  * 不需登入
  * request 
  ```
  {
    Account:string,
    Password:string
  }
  ```
  * response 
  ```
  { 
    "success": true,
    "msg": null,
     "data": {
        "Token": "String",
        "Intrest": [
            "炭烤",
            "日式",
            "家常料理"
        ]
    }
  }
  ```

* POST /api/Register
  * 註冊
  * 不須登入
  * request 
  ```
  {
    "newMember": {
        "account": string,
        "password": string,
        "mName": string,
        "email": string
    },
    "interest": [
        string
    ]
  }
  ```
  * response (成功的話)
  ```
  { 
    "success": true,
    "msg": null,
    "data": "登入成功"
  }
  ```

* POST /api/RegisterValidate
  * 認證會員
  * 不須登入
  * request 
  ```
  {
  "Account": "string",
  "authCode": "string"
  }
  ```
  * response (成功的話)
  ```
  {
    "success": true,
    "msg": null,
    "data": "驗證成功，請去登入"
  }
  ```

## fridge 關於冰箱類的api(必須登入)
* GET /api/fridge/List
  * 列出所有當下登入使用者的冰箱
  * request
    ``` 
    null
    ```
  * response (成功的話)
    ```
    {
      "success": true,
      "msg": null,
      "data": [
        {
            "fId": 2,
            "fName": "我愛的冰箱"
        },
        {
            "fId": 3,
            "fName": "學校的冰箱"
        },
        {
            "fId": 5,
            "fName": "家裡冰箱"
        }
      ]
    }
    ```
* POST /api/fridge/Create
  * 建立冰箱和新增自己以外的會員(Owners可以不填)
  * request
    ``` 
    {
    "fName": "string"
    "Owners": [
        "testtest002",
        "testtest003"
    ]
    }
    ```
  * response (成功的話)
    ```
    {
      "success": true,
      "msg": null,
      "data": "冰箱建立成功"
    }
    ```

* POST /api/fridge/Edit/{fridgeid}
  * 修改指定冰箱的名稱或是成員
  * request
    ``` 
    {
      "fName": "string",
      "user": "string"
    }
    ```
  * response (成功的話)
    ```
    {
      "success": true,
      "msg": null,
      "data": "編輯完成"
    }
    ```

* GET /api/fridge/GetOwner/{fridgeid}
  * 取得指定冰箱的使用者
  * request
    ``` 
    null
    ```
  * response (成功的話)
    ```
    {
      "success": true,
      "msg": null,
      "data": [ memberId ]
    }
    ```

* GET /api/fridge/OwnerDelete/{fid}/{userid}
  * 刪除冰箱的指定擁有者
  * request
    ``` 
    null
    ```
  * response (成功的話)
    ```
    {
      "success": true,
      "msg": null,
      "data": [ 刪除成功 ]
    }
    ```

* GET /api/fridge/Delete/{fridgeid}
  * 刪除冰箱
  * request
    ``` 
    null
    ```
  * response (成功的話)
    ```
    {
      "success": true,
      "msg": null,
      "data": "刪除成功"
    }
    ```

* GET /api/fridge/GetAllFridgeData
  * 列出所有擁有的冰箱和每個冰箱的使用者與食物
  * request
    ``` 
    null
    ```
  * response (成功的話)
    ```
    {
    "success": true,
    "msg": null,
    "data": [
        {
            "fname": "api改名字",
            "members": [
                "testtest006",
                "testtest003"
            ],
            "food": [
                {
                    "id": 24,
                    "fridge_id": 10,
                    "food_name": "有錢錢2",
                    "price": 200,
                    "photo": "hqdefault.jpg",
                    "type": "其他",
                    "expire_date": "2021-08-15T00:00:00",
                    "comment": "null"
                },
                {
                    "id": 25,
                    "fridge_id": 10,
                    "food_name": "有錢錢2",
                    "price": 200,
                    "photo": "hqdefault.jpg",
                    "type": "其他",
                    "expire_date": "2021-08-15T00:00:00",
                    "comment": "null"
                }
            ]
        },
        {
            "fname": "string",
            "members": [
                "testtest006"
            ],
            "food": [
              {
                    "id": 24,
                    "fridge_id": 10,
                    "food_name": "有錢錢2",
                    "price": 200,
                    "photo": "hqdefault.jpg",
                    "type": "其他",
                    "expire_date": "2021-08-15T00:00:00",
                    "comment": "null"
                },
                {
                    "id": 25,
                    "fridge_id": 10,
                    "food_name": "有錢錢2",
                    "price": 200,
                    "photo": "hqdefault.jpg",
                    "type": "其他",
                    "expire_date": "2021-08-15T00:00:00",
                    "comment": "null"
                }
            ]
        }
      ]
    }
    ```

## Food 關於食物類的api
* GET /api/food/Getfood/{fridgeId}
  * 取得指定冰箱中的所有食物
  * request
    ``` 
    null
    ```
  * response (成功的話)
    ```
    {
    "success": true,
    "msg": null,
    "data": {
        "fridgeId": 10,
        "FridgeName": "api改名字",
        "food": [
            {
                "id": 21,
                "fridge_id": 10,
                "food_name": "有錢錢",
                "price": 200,
                "photo": null,
                "type": "其他",
                "expire_date": "2021-08-15T00:00:00",
                "comment": "null"
            }
        ]
      }
    }
    ```
* GET /api/food/GetFoodByPage/{fridgeId}?page={page}
  * 依照每頁五筆食物取得指定冰箱中的食物
  * request
    ``` 
    null
    ```
  * response (成功的話)
    ```
    {
    "success": true,
    "msg": null,
    "data": {
        "fridgeId": 10,
        "FridgeName": "api改名字",
        "food": [
            {
                "id": 21,
                "fridge_id": 10,
                "food_name": "有錢錢",
                "price": 200,
                "photo": null,
                "type": "其他",
                "expire_date": "2021-08-15T00:00:00",
                "comment": "null"
            }
        ]
      }
    }
    ```

* GET /api/food/expire
  * 取得過期食物
  * request
    ``` 
    null
    ```
  * response (成功的話)
    ```
    {
    "success": true,
    "msg": null,
    "data": [
        { 
            "FoodId": 4,
            "FridgeId": 2,
            "fName": "我愛的冰箱",
            "expire_date": "2021-06-14T00:00:00",
            "food_name": "高麗菜"
        },
        {
            "FoodId": 4,
            "FridgeId": 2,
            "fName": "我愛的冰箱",
            "expire_date": "2021-06-24T00:00:00",
            "food_name": "小白菜"
        }
      ]
    }
    ```

* GET /api/food/search/{fridgeId}?search={搜尋字串}
  * 搜尋指定冰箱的食物
  * request
    ``` 
    null
    ```
  * response (成功的話)
    ```
    {
    "success": true,
    "msg": null,
    "data": {
        "fridgeId": 2,
        "FridgeName": "我愛的冰箱",
        "food": [
            {
                "id": 2,
                "fridge_id": 2,
                "food_name": "小白菜",
                "price": 15,
                "photo": "14_elevator.png",
                "type": "蔬果",
                "expire_date": "2021-06-24T00:00:00",
                "comment": null
            }
        ]
      }
    }
    ```

* POST /api/food/Create/{fridgeId}
  * 新增食物進冰箱 
  * 注意: 請使用 Content-Type:multipart/form-data 
  * 必要參數(名稱food_name、類別type、過期日expire_date)
  * 可以不填(照片photo、價格price、附註comment)
  * header 
    * 請把Content-Type改為
  ```
  {
    Content-Type:multipart/form-data
  }
  ```
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
    "data": 新增食物成功
  }
  ```
  
* GET /api/food/Delete/{fridgeid}/{foodid}
  * 刪除指定冰箱中的指定食物
  * request
    ``` 
    null
    ```
  * response (成功的話)
    ```
    {
      "success": true,
      "msg": null,
      "data": 完成刪除
    }
    ```

## Recipe 關於食譜類的api
GET /api/recipe/List
* 取得每頁六筆的食譜資料
  * request
    ``` 
    null
    ```
  * response (成功的話)
    ```
    {
      "success": true,
      "msg": null,
      "data": [ memberId ]
    }
    ```

POST /api/recipe/Create
* 新增食譜 返回新增的該食譜的id， 注意:請使用Content-Type:multipart/form-data
  * header 
    * 請把Content-Type改為
    ```
    {
      Content-Type:multipart/form-data
    }
    ```
  * request
    ``` 
    recipe_name:string
    step:string
    photo:file
    ```
  * response (成功的話)
    ```
    {
      "success": true,
      "msg": null,
      "data": {
        "RecipeId": 10
      }
    }
    ```
POST /api/recipe/CreateIngrident/{recipeId}
* 新增食材
  * request
    ``` 
    { 
      "ingredient": [
        { 
		"name": "隨便", 
        "amount": "一個"
        },
        { 
		"name": "大便", 
        "amount": "一個"
        }
      ]
    }
    ```
  * response (成功的話)
    ```
    {
      "success": true,
      "msg": null,
      "data": "新增食材成功"
    }
    ```
