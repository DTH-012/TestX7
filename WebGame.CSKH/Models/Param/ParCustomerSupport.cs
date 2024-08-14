using System;
using System.Collections.Generic;

namespace MsWebGame.CSKH.Models.Param
{
    public class ParCustomerSupport
    {
        public int SearchType { get; set; }
        public string Value { get; set; }
        public string NickName { get; set; }
        public string PartnerName { get; set; }
        public int TransType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ServiceID { get; set; }
        public int GameID { get; set; }
        public long? SpinID { get; set; }
        public int OrderBy { get; set; }
    }
    public class PartUser
    {
        public int SearchType { get; set; }
        public string Value { get; set; }
        public string NickName { get; set; }
        public string PartnerName { get; set; }
        public int TransType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int ServiceID { get; set; }
        public int GameID { get; set; }
        public long? SpinID { get; set; }
        public int OrderBy { get; set; }
    }
    public class ParConfigLive
    {
        public long UserId { get; set; }
        public List<string> Tranfer { get; set; } = new List<string>();
        public List<string> Game { get; set; } = new List<string>();
        public byte Type { get; set; } = 0;
    }
    public class ParUpdatePhone
    {
        public int ServiceID { get; set; }
        public long AccountId { get; set; }
        public string PhoneNumber { get; set; }
        public int Type { get; set; } = 1;
    }
    public class ParBankGame
    {
        public string GameName { get; set; }
        public int GameId { get; set; }
        public int RoomId { get; set; }
    }
    public class ParBankGameForm
    {
        public int GameId { get; set; }
        public int RoomId { get; set; }
        public string Action { get; set; } = "";
        public long PrizeFund { get; set; } = 0;
        public long JackpotFund { get; set; } = 0;
        public long TotalBank { get; set; } = 0;
        public long PrizeValue { get; set; } = 0;
        public long Balance { get; set; } = 0;
        public long TotalAddBalance { get; set; } = 0;
    }
    public class CasoutBankPar
    {
        public long UserId { get; set; }
        public long Id { get; set; }
        public long ReqID { get; set; }
        public int Status { get; set; }
        public string BankCode { get; set; }
    }
    public class CasinBankPar
    {
        public long UserID { get; set; }
        public long Id { get; set; }
        public long ReqID { get; set; }
        public int Status { get; set; }
        //public string Content { get; set; }
       // public string RequestUser { get; set; }
    }
    public class CasinMomoPar
    {
        public long UserID { get; set; }
        public long Id { get; set; }
        public long ReqID { get; set; }
        public int Status { get; set; }
        //public string Content { get; set; }
        // public string RequestUser { get; set; }
    }
    public class CasinVtPayPar
    {
        public long UserID { get; set; }
        public long Id { get; set; }
        public long ReqID { get; set; }
        public int Status { get; set; }
        //public string Content { get; set; }
        // public string RequestUser { get; set; }
    }
    public class JackpotSetPar
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int RoomId { get; set; }
        public string Nickname { get; set; }
        public int Count { get; set; }
    }
    public class BankOutPar
    {
        public long UserId { get; set; }
        public long Id { get; set; }
        public long ReqID { get; set; }
        public int Status { get; set; }
        public string BankNumber { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
    }
}