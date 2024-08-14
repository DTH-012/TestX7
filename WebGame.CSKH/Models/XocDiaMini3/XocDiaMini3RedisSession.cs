using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace MsWebGame.CSKH.Models.XocDiaMini3
{
    public class XocDiaMini3RedisSession
    {
        public long SessionID { get; set; }

        public int Ellapsed { get; set; }


        public XocDiaMini3RedisSession() { }

        public XocDiaMini3RedisSession(long sessionId, int ellapsed)
        {
            this.SessionID = sessionId;
            this.Ellapsed = ellapsed;

        }
    }
}