所有的都必須在header加上這個使這次的請求回傳json
Content-Type: application/json
除了寫不須授權的其餘都要在header加上授權token
Authorization: Bearer {token}' 

**產生token**
----
  傳送帳號密碼並回傳token

* **URL**

  /users/:id

* **Method:**

  `GET`
  
*  **URL Params**

   **Required:**
 
   `id=[integer]`

* **Data Params**

  None

* **Success Response:**

  * **Code:** 200 <br />
    **Content:** `{ id : 12, name : "Michael Bloom" }`
 
* **Error Response:**

  * **Code:** 404 NOT FOUND <br />
    **Content:** `{ error : "User doesn't exist" }`

  OR

  * **Code:** 401 UNAUTHORIZED <br />
    **Content:** `{ error : "You are unauthorized to make this request." }`

* **Sample Call:**

  ```javascript
    $.ajax({
      url: "/users/1",
      dataType: "json",
      type : "GET",
      success : function(r) {
        console.log(r);
      }
    });
  ```
* 取得token
method:POST
path: api/Token
不須授權
請求參數
{
  Account:string
  Password:string
}
回應
{
  result: 成功登入回true 失敗則false
  token: string
}

* 測試用
method:GET
path: api/home
須授權
請求參數
{
無
}
回應
{
 value1: 成功為 現在登入之使用者id 失敗則 B
 value2: value2
}

