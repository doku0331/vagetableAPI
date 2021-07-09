請自行在專案中加入以下檔案  
App_Data/dbVagetableBasket.mdf  
Services/setting.json

setting.json格式為  
{
  "gmail_account": "寄出驗證信的信箱帳號",  
  "gmail_password": "寄出驗證信的信箱密碼",  
  "gmail_mail": "寄出驗證信的信箱地址"  
}  


所有的都必須在header加上這個使這次的請求回傳json  
Content-Type: application/json  
除了寫不須授權的其餘都要在header加上授權token  
Authorization: Bearer {token}   
