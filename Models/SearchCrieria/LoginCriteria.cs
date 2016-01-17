using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RankDit.Models
{
    public class LoginDataCriteria
    {
        public string Password { get; set; }
        public string Email { get; set; }
        public string DeviceToken { get; set; }
        public string HeaderIsAutoLogin { get; set; }
         public bool IsEncrypted { get; set; }
        public bool IsAccout { get; set; }

        public LoginDataCriteria()
        {
            this.IsAccout = true;
            this.IsEncrypted = true;
        }
    }
}
