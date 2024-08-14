using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace MsWebGame.CSKH.Models.XocDiaMini2
{
    public class XocDiaMini2RedisSession
    {
        public long SessionID { get; set; }

        public int Ellapsed { get; set; }


        public XocDiaMini2RedisSession() { }

        public XocDiaMini2RedisSession(long sessionId, int ellapsed)
        {
            this.SessionID = sessionId;
            this.Ellapsed = ellapsed;

        }
    }
}