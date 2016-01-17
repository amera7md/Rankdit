using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RankDit.Models
{
    public static class  Enum
    {
     public   enum CheckTokenResult
        {
            None=0,
            
            Register=2,
            ErrorDidChangeToday=3,
            ErrorInvalidPassword=4,
            UserDoesntExist=5,
            OK = 6,
        };
    }
}
