所有的都必須在header加上這個使這次的請求回傳json
Content-Type: application/json
除了寫不須授權的其餘都要在header加上授權token
Authorization: Bearer {token}' 


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

