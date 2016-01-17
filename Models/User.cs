using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankDit.Models
{
    [Table("Users")]
    public class User
    {
        public int UserID{get;set;}
        public string Email{get;set;}
        public string Password{get;set;}
        public DateTime CreatedOnDate{get;set;}
        public DateTime? ModefiedOnDate {get;set;}
        public User()
        {
        }
    }
}
