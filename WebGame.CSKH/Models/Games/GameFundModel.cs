using System.ComponentModel;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Models.Games
{
    public class GameFundModel
    {
        public int GameID { get; set; }
        [DisplayName("Phòng")]
        public int RoomID { get; set; }
       
        public long? PrizeFund { get; set; }
       
        public long? JackpotFund { get; set; }
        [DisplayName("Quỹ")]
        public string PrizeFundFormat
        {
            get
            {
                return PrizeFund.LongToMoneyFormat();
            }
        }
        [DisplayName("Hũ")]
        public string JackpotFundFormat
        {
            get
            {
                return JackpotFund.LongToMoneyFormat();
            }
        }
        [DisplayName("Tên game")]
        public string GameName { get; set; }
      
    }
}