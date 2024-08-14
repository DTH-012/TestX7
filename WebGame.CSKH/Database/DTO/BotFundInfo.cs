using System.Collections.Generic;

namespace MsWebGame.CSKH.Database.DTO
{
    public class BotFundInfo
    {
        public List<BotFund> LstBotFund { get; set; }
        public string Amount { get; set; }
        public int TypeFund { get; set; }
    }
}