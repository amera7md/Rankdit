using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RankDit.Models
{
    public class AddPostDTO
    {
        public int UserID { get; set; }
        public string UserEmail { get; set; }
        public string Titel { get; set; }
        public string Description { get; set; }
        public string Place { get; set; }
        public string CountryID { get; set; }
        public int RoundID { get; set; }
        public string VideoData { get; set; }

    }
}
