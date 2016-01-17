using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using RankDit.Models;
using RankDit.Services;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace RankDit.Controllers
{
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        // GET: api/Posts
        [HttpGet]
        public string[] Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/Posts
        [HttpPost]
        public void Post(AddPostDTO NewPostData)
        {
            var Data = Request.Headers["VideoData"];
            NewPostData.VideoData = Data;
            PostService.SaveNewPost(NewPostData);

        }

        [HttpGet("TestUplaod")]
        public string TestUplaod()
        {
            Video NewVidew = new Video() { IsCoverVideo = false, UserEmail = "amer", VideoID = "asdfas" };
           return PostService.UploadVideo(NewVidew);
        }

        [HttpGet("CoverVideo")]
        public string CoverVideo(string UserEmail,int? UserID)
        {
            if(string.IsNullOrEmpty(UserEmail))
              UserEmail = Request.Headers["UserEmail"];
            if(UserID==null)
              UserID = int.Parse( Request.Headers["UserID"]);
            var VideoPath= PostService.SaveNewVideo(true, UserID.Value, UserEmail).VideoPath;
            return VideoPath;
        }
        [HttpGet("CoverPhoto")]
        public string CoverPhoto(string UserEmail, int? UserID)
        {
            if (string.IsNullOrEmpty(UserEmail))
                UserEmail = Request.Headers["UserEmail"];
            if (UserID == null)
                UserID = int.Parse(Request.Headers["UserID"]);
            var VideoPath = PostService.SaveNewPhoto( UserID.Value, UserEmail).PhotoPath;
            return VideoPath;
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
