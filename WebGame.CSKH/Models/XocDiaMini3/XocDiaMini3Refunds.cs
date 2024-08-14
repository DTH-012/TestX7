using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.XocDiaMini3
{
    public class XocDiaMini3Refunds
    {
        public long AccountID { get; set; }
        public long SessionID { get; set; }
        public long Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}