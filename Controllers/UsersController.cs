using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using RankDit.DbContext;
using RankDit.Models;
using RankDit.Services;
 using System.Net;
using Microsoft.Net.Http;
 namespace RankDit.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
           
                UsersContext UserCon=new UsersContext();
            User myUser=UserCon.Users.FirstOrDefault();
           
            return new string[] { myUser.Email,myUser.Password};

        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "Request not Exist.";
        }


        [HttpGet("UserLogin")]
        public IActionResult UserLogin(string UserEmail ,string Password,string DeviceToken)
        {
            try
            {
                if (Request.Headers["UserEmail"] != null)
                    UserEmail = Request.Headers["UserEmail"];
                if (Request.Headers["Password"] != null)
                    Password = Request.Headers["Password"];
                if (Request.Headers["DToken"] != null)
                    DeviceToken = Request.Headers["DToken"];

                var IsAccount = Request.Headers["IsAccount"];// 0=social network
                var headerIsAutoLogin = Request.Headers["IsAutoLogin"];
                if (headerIsAutoLogin == null)
                    headerIsAutoLogin = "";
                //  var DecKey = Request.Headers["Key"];

                LoginDataCriteria LoginData = new LoginDataCriteria() {
                    IsAccout =Boolean.Parse( IsAccount), HeaderIsAutoLogin = headerIsAutoLogin, Password = Password, Email = UserEmail, DeviceToken = DeviceToken };
                
                //Decryption Block........
               //   UserEmail = UserService.DecryptText(UserEmail, DecKey);
               // Password = UserService.DecryptText(Password, DecKey);
               
                var DataResult = UserService.UserLogin(LoginData);

                return new ObjectResult(DataResult);

            }
            catch(Exception Ed)
            {
                return new ObjectResult(Ed.Message);
            }


        }

         
        [HttpGet("Register")]
        public IActionResult Register(string UserEmail, string Password,string DeviceToken)
        {
            try
            {
                //get Request Headers
                if(Request.Headers["UserEmail"]!=null)
                  UserEmail = Request.Headers["UserEmail"];
                if (Request.Headers["Password"]!=null)
                Password = Request.Headers["Password"];
                if (Request.Headers["DeviceToken"] != null)
                    DeviceToken = Request.Headers["DeviceToken"];

                ////decryption Block
                var DecKey = Request.Headers["Key"];
               // UserEmail = UserService.DecryptText(UserEmail, DecKey);
                //Password = UserService.DecryptText(Password, DecKey);

                var ReturnString = UserService.Register(UserEmail, Password, DeviceToken);
                return new ObjectResult(ReturnString);
            }
            catch(Exception ex)
            {
                return new ObjectResult(ex.Message);
            } 

        }
        [HttpGet("ConfirmRegister")]
        public IActionResult ConfirmRegister(string UserEmail, string Password, string DeviceToken)
        {
            try
            {
                if (Request.Headers["UserEmail"] != null)
                    UserEmail = Request.Headers["UserEmail"];
                if (Request.Headers["Password"] != null)
                    Password = Request.Headers["Password"];
                if (Request.Headers["DeviceToken"] != null)
                    DeviceToken = Request.Headers["DeviceToken"];
                var Data = UserService.SaveNewUser(UserEmail, Password, DeviceToken);

                //  var data=  UserService.LoginUser(UserEmail,null);
                return new ObjectResult(Data);
            }
            catch(Exception ex)
            {
                return new ObjectResult(ex.Message);
            }
           
        }
         

        [HttpGet("RestPassword")] 
        public IActionResult RestPassword(string UserEmail )
        {
            try
            {
                if (Request.Headers["UserEmail"] != null)
                    UserEmail = Request.Headers["UserEmail"];
                var Data = UserService.ResetPassword(UserEmail);

                return new ObjectResult(Data);
            }
            catch(Exception ex)
            {
                return new ObjectResult(ex.Message);
            }
          
        }

        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword(string UserEmail,string OldPassword, string NewPassword)
        {
            try
            {
                if (Request.Headers["UserEmail"] != null)
                    UserEmail = Request.Headers["UserEmail"];
                if (Request.Headers["OldPassword"] != null)
                    OldPassword = Request.Headers["OldPassword"];
                if (Request.Headers["NewPassword"] != null)
                    NewPassword = Request.Headers["NewPassword"];
                var Data = UserService.ChangePassword(UserEmail,OldPassword,NewPassword);

                return new ObjectResult(Data);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message);
            }

        }
        [HttpPost]
        public void Post(AddPostDTO NewPost)
        {
            PostService.SaveNewPost(NewPost);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
