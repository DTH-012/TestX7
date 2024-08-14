using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class GameFunds
    {
        public int GameID { get; set; }
        public int RoomID { get; set; }
        public long? PrizeFund { get; set; }
        public long? JackpotFund { get; set; }
        public string GameName { get; set; }
    }
}