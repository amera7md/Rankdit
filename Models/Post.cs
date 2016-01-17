using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankDit.Models
{
    [Table("Posts")] 
    public class Post
    {
        public string PostID { get; set; }
        public string Titel { get; set; }
        public string Description { get; set; }
        public string Place { get; set; }
        public string CountryID { get; set; }
        public string VideoID { get; set; }
        public bool IsActive { get; set; }
        public int RoundID { get; set; }

        public Post()
        {
        }
    }
}
