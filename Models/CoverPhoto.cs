using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RankDit.Models
{
    [Table("CoverPhotos")]
    public class CoverPhoto
    {
        public int PhotoID { get; set; }
        public int UserID { get; set; }
        public string PhotoPath { get; set; }
        [NotMapped]
        public string UserEmail { get; set; }
    }
}
