using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.Md5LuckyDice
{
    public class Md5Refunds
    {
        public long AccountID { get; set; }
        public long SessionID { get; set; }
        public long Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}