using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.SicboChanLeLuckyDice
{
    public class SicboChanLeRefunds
    {
        public long AccountID { get; set; }
        public long SessionID { get; set; }
        public long Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}