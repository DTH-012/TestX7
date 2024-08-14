using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Controllers;
using MsWebGame.RedisCache.Cache;
using Telerik.Web.Mvc;

[AllowedIP]
public class SicBoController : BaseController
{
    private static string _keyHu = "SicBo";

    [AdminAuthorize(Roles = "ADMIN,MONITOR")]
    public ActionResult Index()
    {
        return View();
    }

    [AdminAuthorize(Roles = "ADMIN,MONITOR")]
    [HttpPost]
    [GridAction(EnableCustomBinding = true)]
    public ActionResult InitSession(GridCommand command)
    {
        string keyHu = CachingHandler.Instance.GeneralRedisKey(_keyHu, "Session");
        RedisCacheProvider _cachePvd = new RedisCacheProvider();
        long SessionID = 0L;
        string Phrase = "";
        int Elapsed = 0;
        if (_cachePvd.Exists(keyHu))
        {
            string Session = _cachePvd.Get<string>(keyHu);
            string[] arrSession = Session.Split('|');
            SessionID = long.Parse(arrSession[0]);
            Phrase = arrSession[1];
            Elapsed = int.Parse(arrSession[2]);
        }
        string ModePhat = CachingHandler.Instance.GeneralRedisKey(_keyHu, "NewResult");
        Dictionary<int, BetLogSicBo[]> data = GetUserBet(SessionID);
        JsonResult jsonResult = new JsonResult();
        jsonResult.Data = new
        {
            SessionID = SessionID,
            Phrase = Phrase,
            Elapsed = Elapsed,
            Result = (_cachePvd.Exists(ModePhat) ? _cachePvd.Get<List<int>>(ModePhat) : new List<int> { 0, 0, 0 }),
            Mode = GetModel(),
            Tai = data[1],
            Xiu = data[2],
            Chan = data[3],
            Le = data[4],
            Doi1 = data[5],
            Doi2 = data[6],
            Doi3 = data[7],
            Doi4 = data[8],
            Doi5 = data[9],
            Doi6 = data[10],
            BaoRandom = data[11],
            Bao1 = data[12],
            Bao2 = data[13],
            Bao3 = data[14],
            Bao4 = data[15],
            Bao5 = data[16],
            Bao6 = data[17],
            Tong4 = data[18],
            Tong5 = data[19],
            Tong6 = data[20],
            Tong7 = data[21],
            Tong8 = data[22],
            Tong9 = data[23],
            Tong10 = data[24],
            Tong11 = data[25],
            Tong12 = data[26],
            Tong13 = data[27],
            Tong14 = data[28],
            Tong15 = data[29],
            Tong16 = data[30],
            Tong17 = data[31],
            Cap12 = data[32],
            Cap13 = data[33],
            Cap14 = data[34],
            Cap15 = data[35],
            Cap16 = data[36],
            Cap23 = data[37],
            Cap24 = data[38],
            Cap25 = data[39],
            Cap26 = data[40],
            Cap34 = data[41],
            Cap35 = data[42],
            Cap36 = data[43],
            Cap45 = data[44],
            Cap46 = data[45],
            Cap56 = data[46],
            Don1 = data[47],
            Don2 = data[48],
            Don3 = data[49],
            Don4 = data[50],
            Don5 = data[51],
            Don6 = data[52]
        };
        return jsonResult;
    }

    [AdminAuthorize(Roles = "ADMIN,MONITOR")]
    [HttpPost]
    public ActionResult SetModel(DataModelSicBo dataModel)
    {
        JsonResult jsonResult;
        if (dataModel.model != 2 && dataModel.model != 0)
        {
            jsonResult = new JsonResult();
            jsonResult.Data = new
            {
                IsOke = false
            };
            return jsonResult;
        }
        string keyHu = CachingHandler.Instance.GeneralRedisKey(_keyHu, "Mode");
        RedisCacheProvider _cachePvd = new RedisCacheProvider();
        ModeDiceResult modeDiceResult = new ModeDiceResult();
        modeDiceResult.Mode = dataModel.model;
        _cachePvd.Set(keyHu, modeDiceResult);
        jsonResult = new JsonResult();
        jsonResult.Data = new
        {
            IsOke = true
        };
        return jsonResult;
    }

    [AdminAuthorize(Roles = "ADMIN,MONITOR")]
    [HttpPost]
    public ActionResult SetDice(DataDice dataModel)
    {
        JsonResult jsonResult;
        if (dataModel.dice1 < 0 || dataModel.dice1 > 6 || dataModel.dice2 < 0 || dataModel.dice2 > 6 || dataModel.dice3 < 0 || dataModel.dice3 > 6)
        {
            jsonResult = new JsonResult();
            jsonResult.Data = new
            {
                IsOke = false
            };
            return jsonResult;
        }
        string keyHu = CachingHandler.Instance.GeneralRedisKey(_keyHu, "Mode");
        RedisCacheProvider _cachePvd = new RedisCacheProvider();
        ModeDiceResult modeDiceResult = new ModeDiceResult();
        modeDiceResult.DiceResult.Dice1 = dataModel.dice1;
        modeDiceResult.DiceResult.Dice2 = dataModel.dice2;
        modeDiceResult.DiceResult.Dice3 = dataModel.dice3;
        modeDiceResult.Mode = 1;
        _cachePvd.Set(keyHu, modeDiceResult);
        jsonResult = new JsonResult();
        jsonResult.Data = new
        {
            IsOke = true
        };
        return jsonResult;
    }

    private ModeDiceResult GetModel()
    {
        string keyHu = CachingHandler.Instance.GeneralRedisKey(_keyHu, "Mode");
        RedisCacheProvider _cachePvd = new RedisCacheProvider();
        if (_cachePvd.Exists(keyHu))
        {
            return _cachePvd.Get<ModeDiceResult>(keyHu);
        }
        return new ModeDiceResult();
    }

    private Dictionary<int, BetLogSicBo[]> GetUserBet(long sessionId)
    {
        RedisCacheProvider _cachePvd = new RedisCacheProvider();
        Array betSideSicBo = Enum.GetValues(typeof(BetSideSicBo));
        Dictionary<int, BetLogSicBo[]> betBetSides = new Dictionary<int, BetLogSicBo[]>();
        foreach (object betSide in betSideSicBo)
        {
            string keyHuUserBet = CachingHandler.Instance.GeneralRedisKey(_keyHu, "UserBet:" + sessionId + ":" + Enum.GetName(typeof(BetSideSicBo), betSide).ToString());
            List<BetLogSicBo> userBets = new List<BetLogSicBo>();
            if (_cachePvd.Exists(keyHuUserBet))
            {
                userBets = _cachePvd.Get<List<BetLogSicBo>>(keyHuUserBet)?.Where((BetLogSicBo c) => !c.Bot).ToList();
            }
            Dictionary<long, BetLogSicBo> betSideLogs = new Dictionary<long, BetLogSicBo>();
            if (userBets != null)
            {
                foreach (BetLogSicBo betLog in userBets)
                {
                    if (betSideLogs.ContainsKey(betLog.AccountID))
                    {
                        betSideLogs[betLog.AccountID].BetValue += betLog.BetValue;
                    }
                    else
                    {
                        betSideLogs[betLog.AccountID] = betLog;
                    }
                }
            }
            betBetSides.Add((int)betSide,(from kvp in betSideLogs select kvp.Value into e orderby e.BetValue descending select e).ToArray());
        }
        return betBetSides;
    }
}

public class BetLogSicBo
{
    public long AccountID { get; set; }

    public long BetValue { get; set; }

    public int BetSide { get; set; }

    public string Nickname { get; set; }

    public bool Bot { get; set; } = true;

    public BetLogSicBo(long accountId, long betVal, int betSide, string Nickname = "Bot", bool Bot = true)
    {
        AccountID = accountId;
        BetValue = betVal;
        BetSide = betSide;
        this.Nickname = Nickname;
        this.Bot = Bot;
    }
}

public class DataModelSicBo
{
    public int model { get; set; }
}

public class DataDice
{
    public int dice1 { get; set; }

    public int dice2 { get; set; }

    public int dice3 { get; set; }
}

public class ModeDiceResult
{
    public int Mode { get; set; }

    public DiceResult DiceResult { get; set; }

    public ModeDiceResult()
    {
        Mode = 0;
        DiceResult = new DiceResult();
    }
}

public class DiceResult
{
    public int Dice1 { get; set; }

    public int Dice2 { get; set; }

    public int Dice3 { get; set; }

    public DiceResult()
    {
        Dice1 = -1;
        Dice2 = -1;
        Dice3 = -1;
    }

    public DiceResult(int dice1, int dice2, int dice3)
    {
        Dice1 = dice1;
        Dice2 = dice2;
        Dice3 = dice3;
    }
}

public enum BetSideSicBo
{
    Tai = 1,
    Xiu,
    Chan,
    Le,
    Doi1,
    Doi2,
    Doi3,
    Doi4,
    Doi5,
    Doi6,
    BaoRandom,
    Bao1,
    Bao2,
    Bao3,
    Bao4,
    Bao5,
    Bao6,
    Tong4,
    Tong5,
    Tong6,
    Tong7,
    Tong8,
    Tong9,
    Tong10,
    Tong11,
    Tong12,
    Tong13,
    Tong14,
    Tong15,
    Tong16,
    Tong17,
    Cap12,
    Cap13,
    Cap14,
    Cap15,
    Cap16,
    Cap23,
    Cap24,
    Cap25,
    Cap26,
    Cap34,
    Cap35,
    Cap36,
    Cap45,
    Cap46,
    Cap56,
    Don1,
    Don2,
    Don3,
    Don4,
    Don5,
    Don6
};

