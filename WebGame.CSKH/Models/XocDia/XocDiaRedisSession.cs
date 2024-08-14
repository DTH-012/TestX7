using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace MsWebGame.CSKH.Models.XocDia
{
    public class XocDiaRedisSession
    {
        public long SessionID { get; set; }

        public int Ellapsed { get; set; }


        public XocDiaRedisSession() { }

        public XocDiaRedisSession(long sessionId, int ellapsed)
        {
            this.SessionID = sessionId;
            this.Ellapsed = ellapsed;

        }
    }
}