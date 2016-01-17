using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RankDit.Models
{
    [Table("DeviceTokens")]
    public class DeviceTokenEntity
    {
        public int TokenID { get; set; }
        public string DeviceToken { get; set; }
        public int AccountID { get; set; }
        public bool IsActive { get; set; }
        public bool DidChangeToday { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string DeviceEmail { get; set; }

    }
}
