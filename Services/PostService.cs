using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet;
using RankDit.Models;
using RankDit.DbContext;
using Amazon;

namespace RankDit.Services
{
    public class PostService
    {
        //private static readonly string AWS_ACCESS_KEY = "AKIAJL64MWP4J6NNRQOA";

        //private static readonly string AWS_SECRET_KEY = "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";
        //private static readonly string BUCKET_NAME = "rankdit";
        private static string BUCKET_NAME {  get; set; }

        private static string  AWS_ACCESS_KEY{ get; set; }
         private static string AWS_SECRET_KEY { get; set; }
        private static IAmazonS3 GetAmazonCleint()
        {
            BUCKET_NAME = Startup.Configuration["AWS:BucketName"];
            AWS_ACCESS_KEY = Startup.Configuration["AWS:AccessKey"];
            AWS_SECRET_KEY = Startup.Configuration["AWS:SecretKey"];
            AmazonS3Config config = new AmazonS3Config() { };
            config.RegionEndpoint = RegionEndpoint.EUCentral1;
            Amazon.S3.IAmazonS3 client = AWSClientFactory.CreateAmazonS3Client(AWS_ACCESS_KEY, AWS_SECRET_KEY, config);
            AWSConfigs.S3UseSignatureVersion4 = true;
            return client;
        }
        public static string UploadPhoto(CoverPhoto CurrentPhoto)
        {
            try
            { 
                var client = GetAmazonCleint();
                String S3_KEY ="CoverPhotos/"+ CurrentPhoto.UserEmail+".jpg"
                    ;

                PutObjectRequest request = new PutObjectRequest();
                request.BucketName = BUCKET_NAME;
                request.Key = S3_KEY;
                request.ContentBody = @"    ftypisom   isomiso2avc1mp41 ÅHmoov   lmvhd              è h                                            @                                ";
                request.CannedACL = S3CannedACL.PublicRead;

                client.PutObject(request);
                var Url = GetItemPublicUrlForVideo(S3_KEY);
                return Url;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static string UploadVideo(Video CurrentVideo)
        { 
                try
                {
                var client = GetAmazonCleint();

                // to get current Bucket or insert new One ...........

                // ListBucketsResponse response = client.ListBuckets();

                //bool found = false;
                //foreach (S3Bucket bucket in response.Buckets)
                //{

                //    if (bucket.BucketName == BUCKET_NAME)
                //    {
                //        found = true;
                //        break;

                //    }
                //}
                //if (found == false)
                //{
                //    client.PutBucket(new PutBucketRequest() { BucketName=BUCKET_NAME}  );
                //}

                String S3_KEY = GetItemKey(CurrentVideo. VideoID,CurrentVideo.UserEmail, CurrentVideo.IsCoverVideo);
                 
                PutObjectRequest request = new PutObjectRequest();
                 request.BucketName=BUCKET_NAME;
                request.Key=S3_KEY;
                request.ContentBody =  @"    ftypisom   isomiso2avc1mp41 ÅHmoov   lmvhd              è h                                            @                                ";
                request.CannedACL = S3CannedACL.PublicRead;
             
                client. PutObject(request);
               
                // to get special item form buket 
                //GetObjectRequest Objectreq = new GetObjectRequest
                //{
                //    BucketName = BUCKET_NAME,
                //    Key = S3_KEY
                //};

                //using (GetObjectResponse objResponse = client.GetObject(Objectreq))
                //{

                //    string dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), S3_KEY);
                //    if (!File.Exists(dest))
                //    {
                //        objResponse.WriteResponseStreamToFile(dest);
                //    }
                //}


                //to get item url
                //var expiryUrlRequest = new GetPreSignedUrlRequest() { BucketName = BUCKET_NAME, Key = S3_KEY, Expires = DateTime.Now.AddDays(1) };
                // string url = client.GetPreSignedURL(expiryUrlRequest);

                var Url = GetItemPublicUrlForVideo(S3_KEY);
                return Url;
                 
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                return amazonS3Exception.Message;
                
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
           
        }
        public static string GetItemKey(string VideoID,string UserEmail,bool IsCoverVideo)
        {
            string S3_Key;
            if (IsCoverVideo)
            {
                S3_Key = "CoverVideos/" + UserEmail + "/CoverVideo" + ".mp4";
            }
            else
            {
                S3_Key = VideoID + ".mp4";

            }
            return S3_Key;
        }
        public static string GetItemPublicUrlForVideo(string key)
        {
            return BUCKET_NAME + ".s3.amazonaws.com/" + key;
        }
         
        public static string SaveNewPost(AddPostDTO PostDTO)
        {
            Post NewPost = new Post();
            //// save New Video block .......

            NewPost.VideoID = SaveNewVideo(false, PostDTO.UserID).VideoID;
            using (var context=new PostContext())
            {
                
                /// save new Post block ..............
                NewPost.PostID = CreateNewID();
                 NewPost.IsActive = true;
                NewPost.Description = PostDTO.Description;
                NewPost.CountryID = PostDTO.CountryID;
                NewPost.RoundID = PostDTO.RoundID;
                NewPost.Titel = PostDTO.Titel;
                NewPost.Place = PostDTO.Place; 
                context.Posts.Add(NewPost);

                // save changes to the DB ...........
                context.SaveChanges();
            }
            return "";
        }

        public static Video SaveNewVideo(bool IsCoverVideo,int UserID,string UserEmail="")
        {
            try
            {


                Video NewVideo = new Video();

                using (var context = new PostContext())
                {
                    // save New Video block .......
                    NewVideo.CreatedOnDate = DateTime.Now;
                    NewVideo.IsCoverVideo = IsCoverVideo;
                    NewVideo.ModifiedOnDate = null;
                    NewVideo.VideoID = CreateNewID();
                    NewVideo.UserEmail = UserEmail;
                    NewVideo.VideoPath = UploadVideo(NewVideo);
                    NewVideo.UserID = UserID;
                    context.Videos.Add(NewVideo);
                    context.SaveChanges();
                }
                return NewVideo;

            }
            catch(Exception ex)
            {
                throw ex;
            }
            }

        public static string CreateNewID()
        {
            var newID = Guid.NewGuid().ToString().Substring(0,15);
            return newID;
        }
        public static CoverPhoto SaveNewPhoto(int UserID,string UserEmail)
        {
            try
            {


                CoverPhoto NewPhoto = new CoverPhoto();

                using (var context = new PostContext())
                {
                    // save New Video block .......

                    NewPhoto.UserID = UserID;
                    NewPhoto.UserEmail = UserEmail;
                    NewPhoto.PhotoPath = UploadPhoto(NewPhoto);
                    context.Photos.Add(NewPhoto);
                    context.SaveChanges();
                }
                return NewPhoto;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetTimeLine(string UserEmail,int Index)
        {
            var CurrentUser = UserService.GetCurrentUser(UserEmail);
            using(var context=new PostContext())
            {

            }
            return "";
        }
        public PostService()
        {
        }
    }
}
