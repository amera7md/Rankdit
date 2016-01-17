using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RankDit;
using RankDit.DbContext;
using RankDit.Models;
using RankDit.Models.DTO;
using Microsoft.AspNet.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using static RankDit.Models.Enum;

namespace RankDit.Services
{
    public class UserService
    {
        #region GetCurrentUser
        private static readonly object UnitTableLock = new object();
        private static User CurrentUser;
       // private static bool _ready = false;

        public static User GetCurrentUser(string Email)
        {
            
                        using (UsersContext Context = new UsersContext())
                        {
                            CurrentUser = new User(); //... etc
                              CurrentUser = (from b in Context.Users
                                               where b.Email == Email
                                               select b).FirstOrDefault();

                         
            }

            return CurrentUser;
        }
        public static User GetCurrentUser(string Email,string Password)
        {
            using (UsersContext Context = new UsersContext())
            {
                CurrentUser = new User(); //... etc
                CurrentUser = (from b in Context.Users
                               where b.Email == Email && b.Password==Password
                               select b).FirstOrDefault();


            }
            return CurrentUser;
        }

        //[ThreadStatic]
        //private static User CurraaentUser;
        #endregion

        #region Login
        public static string GetLoginData(LoginDataCriteria LoginData)
        {
            //  IEnumerable<string> ReturnString=new string[]{};


            using (UsersContext Context = new UsersContext())
            {

                var text = "amer";
                string DcriptedPassword = LoginData. Password;
                if (LoginData.IsEncrypted)
                    DcriptedPassword = UserService.DecryptText(LoginData.Password, text);
                User CurrentUser;
                if (LoginData.IsAccout)
                  CurrentUser = GetCurrentUser(LoginData.Email, DcriptedPassword);
                else
                    CurrentUser = GetCurrentUser(LoginData.Email);


                // check if the login with username and password match
                if (CurrentUser == null) return "Login Faild";

                var CurrentAccount = (from Acc in Context.Accounts
                                      where Acc.UserID == CurrentUser.UserID
                                      select Acc).FirstOrDefault();
                var DTO = new UserLoginDTO()
                {
                    FirstName = CurrentAccount.FirstName != null ? CurrentAccount.FirstName : ""
                    ,
                    MiddelName = CurrentAccount.MiddelName != null ? CurrentAccount.MiddelName : ""
                    ,
                    LastName = CurrentAccount.LastName != null ? CurrentAccount.LastName : "",
                    Gender = CurrentAccount.Gender.HasValue ? CurrentAccount.Gender.Value.ToString() : ""
                    ,
                    Email = CurrentUser.Email,
                    Password = CurrentUser.Password != null ? CurrentUser.Password : "",
                    CreatedOnDate = CurrentUser.CreatedOnDate.ToString("dd/MM/yyyy"),
                    BirthDay = CurrentAccount.BirthDay.HasValue ? CurrentAccount.BirthDay.Value.ToString("dd/MM/yyyy") : "",
                    UserID = CurrentUser.UserID,
                    AccountID = CurrentAccount.AccountID,
                    BadgeID = CurrentAccount.BadgeID != null ? CurrentAccount.BadgeID : 0,
                    CountryID = CurrentAccount.CountryID != null ? CurrentAccount.CountryID : "",
                    ModefiedOnDate = CurrentAccount.ModefiedOnDate.HasValue ? CurrentAccount.ModefiedOnDate.Value.ToString("dd/MM/yyyy") : ""
                    ,
                    IsAutoLogin = LoginData.HeaderIsAutoLogin != null ? LoginData.HeaderIsAutoLogin.ToString() : ""
                   ,
                    Points = CurrentAccount.Points.HasValue ? CurrentAccount.Points.Value : 0
                };
                return JsonConvert.SerializeObject(DTO);
            }
 

        }

        public static string UserLogin(LoginDataCriteria LoginData)
        {
            try
            {
              

                DeviceTokenEntity CurrentDeviceTocken = new DeviceTokenEntity() { DeviceToken = LoginData.DeviceToken };
                CheckTokenResult CheckTokenResult = CheckTokenResult.None;
                if ( LoginData.IsAccout)
                    CheckTokenResult = UserService.CheckTokenForLogin(CurrentDeviceTocken, LoginData.Email, LoginData.Password);
                else
                    CheckTokenResult = UserService.CheckTokenForSocialMediaLogin(CurrentDeviceTocken, LoginData.Email);
                if ( CheckTokenResult== CheckTokenResult.OK)
                {
                    LoginData.IsEncrypted = false;
                    string Data = UserService.GetLoginData(LoginData);

                    return Data;

                }
                else
                {
                    if (CheckTokenResult == CheckTokenResult.Register)
                        return UserService.SaveNewUser(LoginData.Email, null, LoginData.DeviceToken);
                    else

                        return CheckTokenResult.ToString();
                }
            }
            catch (Exception ex)
            {
                //error in the code
                return ex.Message;
            }
        }

        #endregion

        #region Register 

        public static string SaveNewToken(int AccountID,string DeviceToken)
        {
            using (var context = new UsersContext())
            {
                var newToken = context.Tokens.Add(new DeviceTokenEntity() { DeviceToken = DeviceToken, DidChangeToday = false, IsActive = true, AccountID =AccountID });
                context.SaveChanges();
                return DeviceToken;
            }
        }
        public static CheckTokenResult CheckTokenForLogin(DeviceTokenEntity DeviceTokenEntity, string SentEmail,string Password )
        {
            using (var Context = new UsersContext())
            {
                DeviceTokenEntity CurrentToken = new DeviceTokenEntity();
                User CurrentUser = new User();
                if (Context.Tokens.Where(p => p.DeviceToken == DeviceTokenEntity.DeviceToken).Any())
                {
                    CurrentToken = Context.Tokens.Where(p => p.DeviceToken == DeviceTokenEntity.DeviceToken).FirstOrDefault();
                    var EmailCurrentToken = "";
                    if (CurrentToken != null)
                    {
                        var deviceUser = (from acc in Context.Accounts
                                          where
                    CurrentToken.AccountID == acc.AccountID
                                          select acc.UserID).FirstOrDefault();

                        EmailCurrentToken = (from Usr in Context.Users where Usr.UserID == deviceUser select Usr.Email).FirstOrDefault();
                    }
                    if (EmailCurrentToken == SentEmail)
                    {
                      //  if(!string.IsNullOrEmpty(Password))
                        CurrentUser = GetCurrentUser(SentEmail,Password);
                       // else
                         //   CurrentUser = GetCurrentUser(SentEmail);// social media login 

                        if (CurrentUser != null) return CheckTokenResult.OK;
                        else return CheckTokenResult.ErrorInvalidPassword;

                    }
                    else
                    {
                        if (CurrentToken.DidChangeToday)
                            return CheckTokenResult.ErrorDidChangeToday;
                        else
                        {
                            if (!string.IsNullOrEmpty(Password))
                                CurrentUser = GetCurrentUser(SentEmail, Password);
                            else
                                CurrentUser = GetCurrentUser(SentEmail);
                            if(CurrentUser!=null)
                            { 
                                    CurrentToken.DidChangeToday = true;

                                    Context.Tokens.Attach(CurrentToken);
                                    var entry = Context.Entry(CurrentToken);
                                    entry.Property(e => e.DidChangeToday).IsModified = true; 
                                    Context.SaveChanges(); 
                                     return CheckTokenResult.OK; 
                            }
                            else
                            {
                                return CheckTokenResult.ErrorInvalidPassword;
                            }

                        }
                    }
 
              
                }
                else
                {
                    CurrentUser = GetCurrentUser(SentEmail);
                    if (CurrentUser == null)
                        return CheckTokenResult.UserDoesntExist;
                    else
                    {
                         
                        CurrentUser = GetCurrentUser(SentEmail, Password);
                         
                        if (CurrentUser == null) 
                        return CheckTokenResult.ErrorInvalidPassword;
                        else
                        {
                            Account CurrentAccount = Context.Accounts.Where(p => p.UserID == CurrentUser.UserID).FirstOrDefault();

                            var NewToken = UserService.SaveNewToken(CurrentAccount.AccountID, DeviceTokenEntity.DeviceToken);
                            var newTokenEntity = new DeviceTokenEntity() { AccountID = CurrentAccount.AccountID, DeviceToken = NewToken, DeviceEmail = null };
                            return CheckTokenResult.OK;
                        }
                            
                        
                    }

                }


#pragma warning disable CS0162 // Unreachable code detected
                return CheckTokenResult.None;
#pragma warning restore CS0162 // Unreachable code detected
            }

        }


        public static CheckTokenResult CheckTokenForSocialMediaLogin(DeviceTokenEntity DeviceTokenEntity, string SentEmail)
        {
            DeviceTokenEntity CurrentToken = new DeviceTokenEntity();
            User CurrentUser = new User();
            using (var Context = new UsersContext())
            {
                if (Context.Tokens.Where(p => p.DeviceToken == DeviceTokenEntity.DeviceToken).Any())
                {
                    CurrentToken = Context.Tokens.Where(p => p.DeviceToken == DeviceTokenEntity.DeviceToken).FirstOrDefault();
                    var EmailCurrentToken = "";
                    if (CurrentToken != null)
                    {
                        var deviceUser = (from acc in Context.Accounts
                                          where
                    CurrentToken.AccountID == acc.AccountID
                                          select acc.UserID).FirstOrDefault();

                        EmailCurrentToken = (from Usr in Context.Users where Usr.UserID == deviceUser select Usr.Email).FirstOrDefault();
                    }
                    if (EmailCurrentToken == SentEmail)
                    {
                         CurrentUser = GetCurrentUser(SentEmail);
                        
                        if (CurrentUser != null) return CheckTokenResult.OK;
                        else return CheckTokenResult.Register;

                    }
                    else
                    {
                        if (CurrentToken.DidChangeToday)
                            return CheckTokenResult.ErrorDidChangeToday;
                        else
                        {
                           
                             
                                CurrentUser = GetCurrentUser(SentEmail);
                            if (CurrentUser != null)
                            {
                                CurrentToken.DidChangeToday = true;

                                Context.Tokens.Attach(CurrentToken);
                                var entry = Context.Entry(CurrentToken);
                                entry.Property(e => e.DidChangeToday).IsModified = true;
                                Context.SaveChanges();
                                return CheckTokenResult.OK;
                            }
                            else
                            {
                                return CheckTokenResult.Register;
                            }

                        }
                    }
                }else
                {
                    CurrentUser = GetCurrentUser(SentEmail);
                    if (CurrentUser == null)
                        return CheckTokenResult.Register;
                    else
                    {

 
                     
                            Account CurrentAccount = Context.Accounts.Where(p => p.UserID == CurrentUser.UserID).FirstOrDefault();

                            var NewToken = UserService.SaveNewToken(CurrentAccount.AccountID, DeviceTokenEntity.DeviceToken);
                            var newTokenEntity = new DeviceTokenEntity() { AccountID = CurrentAccount.AccountID, DeviceToken = NewToken, DeviceEmail = null };
                            return CheckTokenResult.OK;
                        


                    }
                }


            }

        }
        public static DeviceTokenEntity CheckTokenForRegister(DeviceTokenEntity DeviceTokenEntity, string SentEmail)
        {
            using (var Context = new UsersContext())
            {
                DeviceTokenEntity CurrentToken = new DeviceTokenEntity();
               User CurrentUser = GetCurrentUser(SentEmail);
                if (CurrentUser != null)
                {
                    Account CurrentAccount = Context.Accounts.Where(p => p.UserID == CurrentUser.UserID).FirstOrDefault();
                    // CurrentToken = Context.Tokens.Where(p => p.DeviceToken == DeviceTokenEntity.DeviceToken && p.AccountID == CurrentAccount.AccountID).Any();
                    if (Context.Tokens.Where(p => p.DeviceToken == DeviceTokenEntity.DeviceToken && p.AccountID == CurrentAccount.AccountID).Any())
                    {
                        CurrentToken = Context.Tokens.Where(p => p.DeviceToken == DeviceTokenEntity.DeviceToken && p.AccountID == CurrentAccount.AccountID).FirstOrDefault();
                        CurrentToken.DeviceEmail = null;
                        return CurrentToken;
                    }
                    else // Existed Account login with new Device
                    {
                        var NewToken = UserService.SaveNewToken(CurrentAccount.AccountID, DeviceTokenEntity.DeviceToken);
                        var newTokenEntity = new DeviceTokenEntity() { AccountID = CurrentAccount.AccountID, DeviceToken = NewToken, DeviceEmail = null };
                        return newTokenEntity;
                    }

                }
                else
                {
                    CurrentToken = Context.Tokens.Where(p => p.DeviceToken == DeviceTokenEntity.DeviceToken).FirstOrDefault();
                    if (CurrentToken != null)
                    {
                        var deviceUser = (from acc in Context.Accounts
                                          where
                    CurrentToken.AccountID == acc.AccountID
                                          select acc.UserID).FirstOrDefault();

                        CurrentToken.DeviceEmail = (from Usr in Context.Users where Usr.UserID == deviceUser select Usr.Email).FirstOrDefault();
                        return CurrentToken;

                    }
                    return null;
                } 
                return CurrentToken;
            }

        }


        public static bool IsEmailExist(string Email)
        {
            User CurrentUser = GetCurrentUser(Email);
            return (CurrentUser != null); 
        }
        public static string SaveNewUser(string UserEmail, string Password, string DeviceToken)
        {
            try
            {
                using (var context = new UsersContext())
                {
                    // add new User 
                    var NewUser = context.Users.Add(new User() { Email = UserEmail, Password = Password, CreatedOnDate = DateTime.Now });
                    context.SaveChanges();
                    //then add new account 
                    var NewAccount = context.Accounts.Add(new Account() { UserID = NewUser.Entity.UserID, CreatedOnDate = DateTime.Now, ModefiedOnDate = null });
                    context.SaveChanges();
                    //finally add new device token .......
                    var newToken = context.Tokens.Add(new DeviceTokenEntity() { DeviceToken = DeviceToken, DidChangeToday = false, IsActive = true, AccountID = NewAccount.Entity.AccountID });
                    context.SaveChanges();
                    LoginDataCriteria LoginData = new LoginDataCriteria() { Email = UserEmail, Password = Password, HeaderIsAutoLogin = null, IsEncrypted = false };
                    if (Password == null) LoginData.IsAccout = false;
                    return UserService.GetLoginData(LoginData);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public static string Register(string UserEmail, string Password, string DeviceToken)
        {
            DeviceTokenEntity CurrentDeviceTocken = new DeviceTokenEntity() { DeviceToken = DeviceToken };
            var ExistToken = UserService.CheckTokenForRegister(CurrentDeviceTocken, UserEmail);
            if (ExistToken != null)
            {
                if (ExistToken.DeviceEmail != UserEmail)
                {
                    var IsEmailRegister = UserService.IsEmailExist(UserEmail);
                    if (IsEmailRegister)
                        return "Email Allready Registered";
                    else
                    {
                        if (ExistToken.DidChangeToday)
                            return "you have used 2 accounts on this device";
                        else
                            return "this device is bounded to another account ";

                    }
                }
                return "Error Allready Registred Account on this Device";


            }
            else
            {
                if (!UserService.IsEmailExist(UserEmail))
                {
                    var Data = UserService.SaveNewUser(UserEmail, Password, DeviceToken);
                    return Data;
                }
                return "Email Allready Registered";

            }
        }


        #endregion

        #region Change/Rest Password
        public static string ResetPassword(string UserEmail)
        {
            User currentUser = GetCurrentUser(UserEmail);
            using (UsersContext Context = new UsersContext())
            {
                if (currentUser != null)
                {
                    currentUser.Password = GenerateRandomPasswordOrName(10);
                    Context.Users.Attach(currentUser);
                    var entry = Context.Entry(currentUser);
                    entry.Property(e => e.Password).IsModified = true;
                     

                    Context.SaveChanges();
                    return currentUser.Password;
                }
                return "user not registerd";
               
            }

        }

        public static string GenerateRandomPasswordOrName(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();

        }

        public static string ChangePassword(string UserEmail, string OldPassword, string NewPassword)
        {
            User currentUser = GetCurrentUser(UserEmail,OldPassword);
            if (currentUser != null)
            {


                using (UsersContext Context = new UsersContext())
                {

                    currentUser.Password = NewPassword;

                    Context.Users.Attach(currentUser);
                    var entry = Context.Entry(currentUser);
                    entry.Property(e => e.Password).IsModified = true;


                    Context.SaveChanges();
                    return currentUser.Password;
                }
            }
            else {
                return "Error Old password Not Correct";
            }
            
        }
        #endregion

        #region Encript/Decript
        public static string DecryptText(string input, string password)
        {
#if DNX451

            // Get the bytes of the string
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);
           
            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
#endif
#if DNXCORE50
            return input;
#endif
        }
        public static string EncryptText(string input, string password)
        {
#if DNX451

            // Get the bytes of the string
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            string result = Convert.ToBase64String(bytesEncrypted);

            return result;
#endif
#if DNXCORE50
            return input;
#endif
        }
#if DNX451

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }
        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
#endif
        #endregion
        public UserService()
        {
             
        }
    }
}
