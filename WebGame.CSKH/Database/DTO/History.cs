using System;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class HistoryJackpot
    {
        public long AccountID { get; set; }
        public string UserName { get; set; }
        public int GameID { get; set; }
        public string GameIDFormat { get { return GameID.IntToGameFormat(); } }
        public long PrizeValue { get; set; }
        public string PrizeValueFormat { get { return PrizeValue.LongToMoneyFormat(); } }
        public DateTime CreatedTime { get; set; }
    }

    public class HistoryPlay
    {
        public long STT { get; set; }
        public long OrgBalance { get; set; }
        public long Balance { get; set; }
        public string OrgBalanceFormat { get { return OrgBalance.LongToMoneyFormat(); } }
        public string BalanceFormat { get { return Balance.LongToMoneyFormat(); } }
        public long BetValue { get; set; }
        public string BetValueFormat { get { return BetValue.LongToMoneyFormat(); } }
        public long PrizeValue { get; set; }
        public string PrizeValueFormat { get { return PrizeValue.LongToMoneyFormat(); } }
        public long RefundValue { get; set; }
        public string RefundValueFormat { get { return RefundValue.LongToMoneyFormat(); } }
        public long SpinID { get; set; }
        public int GameType { get; set; }
        public string GameTypeFormat { get { return GameType.IntToGameFormat(); } }
        public string Description { get; set; }
        public DateTime PlayTime { get; set; }
        public long AccountID { get; set; }
    }

    public class HistoryTransfer
    {
        public long CreateUserID { get; set; }
        public string CreateUserName { get; set; }
        public string ReceiverName { get; set; }
        public string CreateDisplayName { get; set; }
        public string ReceiverDisplayName { get; set; }
        public long OrgAmount { get; set; }
        public long OrgBalance { get; set; }
        public long Amount { get; set; }
        public DateTime TransDate { get; set; }
        public string TranStatus { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public int ReceiverType { get; set; }

    }

    public class HistoryWalletLog
    {
        public long UserID { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public long OrgBalance { get; set; }
        public string OrgBalanceFormat
        {
            get { return OrgBalance.LongToMoneyFormat(); }
        }
        public long Amount { get; set; }
        public string AmountFormat
        {
            get { return Amount.LongToMoneyFormat(); }
        }
        public long RemainBalance { get; set; }
        public string RemainBalanceFormat
        {
            get { return RemainBalance.LongToMoneyFormat(); }
        }
        public string PartnerName { get; set; }
        public int PartnerType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
    }
    public class HistoryClickJackpot
    {
        public int Id { get; set; }
        public int GameID { get; set; }
        public int RoomID { get; set; }
        public string Nickname { get; set; }
        public int Count { get; set; }
        public DateTime CreateTime { get; set; }
        public string RoomName
        {
            get
            {
                if (RoomID == 1)
                {
                    return "100";
                }
                else if (RoomID == 2)
                {
                    return "1000";
                }
                else if (RoomID == 3)
                {
                    return "5000";
                }
                else if (RoomID == 4)
                {
                    return "10000";
                }
                else
                {
                    return "All";
                }
            }
        }
        public string GameName
        {
            get
            {
                string GameName = "";
                //new KeyValuePair<int, string>(100, "Kho báu tứ linh"), 
                //new KeyValuePair<int, string>(200, "Tây du ký"),
                //new KeyValuePair<int, string>(300, "Thuỷ Cung"),
                //new KeyValuePair<int, string>(400, "Songoku"),
                //new KeyValuePair<int, string>(500, "Ai Cập")

                if (GameID == 100)
                {
                    GameName = "Cowboy";
                }
                else if (GameID == 700)
                {
                    GameName = "MiniPoker";
                }
                else if (GameID == 800)
                {
                    GameName = "Siêu xe";
                }
                else if (GameID == 600)
                {
                    GameName = "Kim Cương";
                }
                else if (GameID == 200)
                {
                    GameName = "Tam Quốc";
                }
                else if (GameID == 300)
                {
                    GameName = "Thuỷ Cung";
                }
                else if (GameID == 400)
                {
                    GameName = "Songoku";
                }
                else if (GameID == 500)
                {
                    GameName = "Ai Cập";
                }
                else if (GameID == 900)
                {
                    GameName = "Panda";
                }
                else if (GameID == 111)
                {
                    GameName = "Zombie";
                }
                else if (GameID == 222)
                {
                    GameName = "Thần Sét";
                }
                else if (GameID == 333)
                {
                    GameName = "SuperHerDes";
                }
                else if (GameID == 444)
                {
                    GameName = "Thần Rừng";
                }
                else if (GameID == 555)
                {
                    GameName = "Gái Nhảy";
                }
                else if (GameID == 666)
                {
                    GameName = "Frozen";
                }
                else if (GameID == 777)
                {
                    GameName = "Super Mario";
                }
                else if (GameID == 888)
                {
                    GameName = "Pokemon";
                }


                else if (GameID == 1)
                {
                    GameName = "Islands";
                }
                else if (GameID == 7)
                {
                    GameName = "KingStar";
                }

                else if (GameID == 14)
                {
                    GameName = "RongHo";
                }
                else if (GameID == 63)
                {
                    GameName = "Sedie";
                }
                else if (GameID == 15)
                {
                    GameName = "Songoku";
                }
                else if (GameID == 2)
                {
                    GameName = "Tayduky";
                }
                else if (GameID == 11)
                {
                    GameName = "Xpoker";
                }
                else if (GameID == 8)
                {
                    GameName = "TaiXiu";
                }
                else if (GameID == 68)
                {
                    GameName = "TaiXiuMd5";
                }
                return GameName;
            }
        }
    }
}