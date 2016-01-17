using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RankDit.Models.DTO
{
    public class UserLoginDTO
    {
        public int AccountID { get; set; }
         public string Gender { get; set; }
        public string FirstName { get; set; }
        public string MiddelName { get; set; }
        public int? Points { get; set; }
        public string LastName { get; set; }
        public string BirthDay { get; set; }
        public string CreatedOnDate { get; set; }
        public string ModefiedOnDate { get; set; }
        public string CountryID { get; set; }
        public int? BadgeID { get; set; }
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string IsAutoLogin { get; set; }
       // public string DidChangeToday { get; set; }
        
    }
}
