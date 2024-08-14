using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace MsWebGame.CSKH.Models.XocDiaMini4
{
    public class XocDiaMini4RedisSession
    {
        public long SessionID { get; set; }

        public int Ellapsed { get; set; }


        public XocDiaMini4RedisSession() { }

        public XocDiaMini4RedisSession(long sessionId, int ellapsed)
        {
            this.SessionID = sessionId;
            this.Ellapsed = ellapsed;

        }
    }
}