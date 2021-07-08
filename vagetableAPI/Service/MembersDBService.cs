using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web;
using vagetableAPI.Models;
// 引進命名空間，給 Hash 用
using System.Security.Cryptography;
// 給 Encoding 物件用
using System.Text;

namespace vagetableAPI.Service
{
    public class MembersDBService
    {
        // 宣告 db 物件 (用來連接 Member 資料表)

        dbVagetableBasketEntities db = new dbVagetableBasketEntities();
        
        // 註冊新會員的方法

        public void Register (Member newMember)
        {
            
            newMember.password = HashPassword(newMember.password);
            db.Member.Add(newMember);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            
        }

        // Hash 密碼用的方法

        public string HashPassword(string password)
        {
            // 宣告 Hash 時所添加的無意義亂數值
            string saltkey = "1q2w3e4r5t6y7u8ui9o0po7tyy";

            // 將密碼與亂數值結合起來
            string saltAndPassword = String.Concat(password, saltkey);

            // 定義 SHA256 的 HASH 物件
            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();

            // 取得密碼轉成 Byte 資料
            byte[] PasswordData = Encoding.Default.GetBytes(saltAndPassword);

            // 取得 Hash 後 Byte 資料
            byte[] HashDate = sha256Hasher.ComputeHash(PasswordData);

            // 將 Hash 後 Byte 資料轉成 String
            string HashResult = Convert.ToBase64String(HashDate);

            // 回傳 Hash 後結果
            return HashResult;
        }

        //藉由帳號取得單筆資料的方法

        private Member GetDataByAccount(string Account)
        {
            Member Data = new Member();
          
            var sql = (from m in db.Member
                      where m.account == Account
                      select m).FirstOrDefault(); 

            // 確保程式不會因執行錯誤而整個中斷
            try
            {
                Data.account = sql.account.ToString();
                Data.password = sql.password.ToString();
                Data.mName = sql.mName.ToString();
                Data.email = sql.email.ToString();
                Data.authCode = sql.authCode.ToString();
            }

            catch (Exception e)
            {
                // 查無資料
                Data = null;
                throw new Exception(e.Message.ToString());
            }
            
            // 回傳根據編號所取得的資料
            return Data;
        }

        // 確認要註冊的帳號是否有被註冊過的方法

        public bool AccountCheck(string Account)
        {
            // 藉由傳入帳號取得會員資料
            Member Data = GetDataByAccount(Account);

            // 判斷是否有查詢到會員
            bool result = (Data == null);

            // 回傳結果
            return result;
        }

        //取得會員資料 (取得非私密性資料(名字&帳號))
        public Member GetDatabyAccount(string Account)
        {
            // 回傳根據帳號所取得的資料
            Member Data = new Member();

            // Linq 語法
            var sql = (from m in db.Member
                       where m.account == Account
                       select m).FirstOrDefault();

            // 確保程式不會因執行錯誤而整個中斷
            try
            {
                Data.mName = sql.mName.ToString();
                Data.account = sql.account.ToString();
               
            }

            catch (Exception e)
            {
                // 沒有資料傳回null
                Data = null;
                throw new Exception(e.Message.ToString());
            }

            return Data;
        }

        // 信箱驗證碼驗證方法
        public string EmailValidate(string Account, string AuthCode)
        {
            // 取得傳入帳號的會員資料
            Member ValidateMember = GetDataByAccount(Account);

            // 宣告驗證後訊息字串
            string ValidateStr = string.Empty;

            if (ValidateMember != null)
            {
                //判斷傳入驗證碼與資料庫中是否相同
                if (ValidateMember.authCode == AuthCode)
                {
                    // 將資料庫中的驗證碼設為空
                    // Linq 更新語法
                    (from m in db.Member where m.account == Account select m)
                    .Single().authCode = string.Empty;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        // 丟出錯誤
                        throw new Exception(e.Message.ToString());
                    }
                    
                    ValidateStr = "帳號信箱驗證成功，現在可以登入了";
                }
                else
                {
                    ValidateStr = "驗證碼錯誤，請重新確認或再註冊";
                }
            }
            else
            {
                ValidateStr = "傳送資料錯誤，請重新確認或再註冊";
            }
            // 回傳驗證訊息
            return ValidateStr;
        }

        // 登入帳密確認方法，並回傳驗證後訊息
        public string LoginCheck(string Account, string Password)
        {
            // 取得傳入帳號的會員資料
            Member LoginMember = GetDataByAccount(Account);

            // 判斷是否有此會員
            if (LoginMember != null)
            {
                // 判斷是否有經過信箱驗證，有經驗證驗證碼欄位會被清空
                if (String.IsNullOrWhiteSpace(LoginMember.authCode))
                {
                    // 進行帳號密碼確認
                    if (PasswordCheck(LoginMember, Password))
                    {
                        return "";
                    }
                    else
                    {
                        return "密碼輸入錯誤";
                    }
                }
                else
                {
                    return "此帳號尚未經過Email驗證，請去收信";
                }
            }
            else
            {
                return "無此會員帳號，請去註冊";
            }
        }

        // 進行密碼確認方法
        public bool PasswordCheck(Member CheckMember, string Password)
        {
            // 判斷資料庫裡的密碼資料與傳入密碼資料Hash後是否一樣
            bool result = CheckMember.password.Equals(HashPassword(Password));

            // 回傳結果
            return result;
        }

        // 變更會員密碼方法，並回傳最後訊息
        public string ChangePassword(string Account, string Password, string newPassword)
        {
            // 取得傳入帳號的會員資料
            Member LoginMember = GetDataByAccount(Account);

            // 確認舊密碼正確性
            if (PasswordCheck(LoginMember, Password))
            {
                // 將新密碼Hash後寫入資料庫中
                LoginMember.password = HashPassword(newPassword);

                // Linq 更新語法
                (from m in db.Member where m.account == Account select m)
                .Single().password = LoginMember.password;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message.ToString());
                }
                
                return "密碼變更成功";
            }
            else
            {
                return "舊密碼輸入錯誤";
            }
        }
    }
}