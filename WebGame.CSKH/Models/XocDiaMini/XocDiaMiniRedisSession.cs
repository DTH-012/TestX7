using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace MsWebGame.CSKH.Models.XocDiaMini
{
    public class XocDiaMiniRedisSession
    {
        public long SessionID { get; set; }

        public int Ellapsed { get; set; }


        public XocDiaMiniRedisSession() { }

        public XocDiaMiniRedisSession(long sessionId, int ellapsed)
        {
            this.SessionID = sessionId;
            this.Ellapsed = ellapsed;

        }
    }
}