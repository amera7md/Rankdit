using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankDit.Models
{
        [Table("Accounts")]

    public class Account
    {
        public int AccountID{get;set;}
        public int UserID{get;set;} 
        public bool? Gender{get;set;}
        public string FirstName{get;set;}
        public string MiddelName{get;set;}
        public int? Points { get; set; }
        public string LastName{get;set;}
        public DateTime? BirthDay{get;set;}
        public DateTime CreatedOnDate{get;set;}
        public DateTime? ModefiedOnDate{get;set;}
        public string CountryID{get;set;}
        public int? BadgeID{get;set;}
        
        public Account()
        {
        }
    }
}
