using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RankDit.Models
{
    [Table("Videos")]
    public class Video
    {
        public  string VideoID { get; set; }
        public int UserID { get; set; }
        public bool IsCoverVideo { get; set; }
        public string VideoPath { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime? ModifiedOnDate { get; set; }
        [NotMapped]
        public string UserEmail { get; set; }
 
    }
}
