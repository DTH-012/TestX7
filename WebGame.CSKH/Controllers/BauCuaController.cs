using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Utils;
using MsWebGame.RedisCache.Cache;
using Newtonsoft.Json;
using MsWebGame.RedisCache.Cache;
namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class BauCuaController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Index()
        {
             
            return View();
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult InitSession(GridCommand command)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("BauCua", "Session");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            long SessionID = 0;
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
            string ModePhat = CachingHandler.Instance.GeneralRedisKey("BauCua", "NewResult");

            string keyHuUserBetChicken = CachingHandler.Instance.GeneralRedisKey("BauCua", "UserBet:" + SessionID+":"+ BetSideBauCua.Chicken);
            List<BetLogBauCua> UserBetChicken = new List<BetLogBauCua>();
            
            if (_cachePvd.Exists(keyHuUserBetChicken))
            {
                UserBetChicken =  _cachePvd.Get<List<BetLogBauCua>>(keyHuUserBetChicken)?.Where(c => c.Bot == false).ToList();
            }
            string keyHuUserBetCrab = CachingHandler.Instance.GeneralRedisKey("BauCua", "UserBet:" + SessionID + ":" + BetSideBauCua.Crab);
            List<BetLogBauCua> UserBetCrab = new List<BetLogBauCua>();

            if (_cachePvd.Exists(keyHuUserBetCrab))
            {
                UserBetCrab = _cachePvd.Get<List<BetLogBauCua>>(keyHuUserBetCrab)?.Where(c => c.Bot == false).ToList();
            }

            string keyHuUserBetDeer = CachingHandler.Instance.GeneralRedisKey("BauCua", "UserBet:" + SessionID + ":" + BetSideBauCua.Deer);
            List<BetLogBauCua> UserBetDeer = new List<BetLogBauCua>();

            if (_cachePvd.Exists(keyHuUserBetDeer))
            {
                UserBetDeer = _cachePvd.Get<List<BetLogBauCua>>(keyHuUserBetDeer)?.Where(c => c.Bot == false).ToList();
            }
            string keyHuUserBetFish = CachingHandler.Instance.GeneralRedisKey("BauCua", "UserBet:" + SessionID + ":" + BetSideBauCua.Fish);
            List<BetLogBauCua> UserBetFish = new List<BetLogBauCua>();

            if (_cachePvd.Exists(keyHuUserBetFish))
            {
                UserBetFish = _cachePvd.Get<List<BetLogBauCua>>(keyHuUserBetFish)?.Where(c => c.Bot == false).ToList();
            }
            string keyHuUserBetGourd = CachingHandler.Instance.GeneralRedisKey("BauCua", "UserBet:" + SessionID + ":" + BetSideBauCua.Gourd);
            List<BetLogBauCua> UserBetGourd = new List<BetLogBauCua>();

            if (_cachePvd.Exists(keyHuUserBetGourd))
            {
                UserBetGourd = _cachePvd.Get<List<BetLogBauCua>>(keyHuUserBetGourd)?.Where(c => c.Bot == false).ToList();
            }
            string keyHuUserBetShrimp = CachingHandler.Instance.GeneralRedisKey("BauCua", "UserBet:" + SessionID + ":" + BetSideBauCua.Shrimp);
            List<BetLogBauCua> UserBetShrimp = new List<BetLogBauCua>();

            if (_cachePvd.Exists(keyHuUserBetShrimp))
            {
                UserBetShrimp = _cachePvd.Get<List<BetLogBauCua>>(keyHuUserBetShrimp)?.Where(c => c.Bot == false).ToList();
            }

            Dictionary<long, BetLogBauCua> Chicken = new Dictionary<long, BetLogBauCua>();
            Dictionary<long, BetLogBauCua> Crab = new Dictionary<long, BetLogBauCua>();
            Dictionary<long, BetLogBauCua> Deer = new Dictionary<long, BetLogBauCua>();
            Dictionary<long, BetLogBauCua> Fish = new Dictionary<long, BetLogBauCua>();
            Dictionary<long, BetLogBauCua> Gourd = new Dictionary<long, BetLogBauCua>();
            Dictionary<long, BetLogBauCua> Shrimp = new Dictionary<long, BetLogBauCua>();
            if (UserBetChicken != null)
            {
                foreach (BetLogBauCua betLog in UserBetChicken)
                {
                    if (Chicken.ContainsKey(betLog.AccountID))
                    {
                        Chicken[betLog.AccountID].BetValue += betLog.BetValue;
                    }
                    else
                    {
                        Chicken[betLog.AccountID] = betLog;
                    }
                }
            }
            if (UserBetCrab != null)
            {
                foreach (BetLogBauCua betLog in UserBetCrab)
                {
                    if (Crab.ContainsKey(betLog.AccountID))
                    {
                        Crab[betLog.AccountID].BetValue += betLog.BetValue;
                    }
                    else
                    {
                        Crab[betLog.AccountID] = betLog;
                    }
                }
            }
            if (UserBetDeer != null)
            {
                foreach (BetLogBauCua betLog in UserBetDeer)
                {
                    if (Deer.ContainsKey(betLog.AccountID))
                    {
                        Deer[betLog.AccountID].BetValue += betLog.BetValue;
                    }
                    else
                    {
                        Deer[betLog.AccountID] = betLog;
                    }
                }
            }
            if (UserBetFish != null)
            {
                foreach (BetLogBauCua betLog in UserBetFish)
                {
                    if (Fish.ContainsKey(betLog.AccountID))
                    {
                        Fish[betLog.AccountID].BetValue += betLog.BetValue;
                    }
                    else
                    {
                        Fish[betLog.AccountID] = betLog;
                    }
                }
            }
            if (UserBetGourd != null)
            {
                foreach (BetLogBauCua betLog in UserBetGourd)
                {
                    if (Gourd.ContainsKey(betLog.AccountID))
                    {
                        Gourd[betLog.AccountID].BetValue += betLog.BetValue;
                    }
                    else
                    {
                        Gourd[betLog.AccountID] = betLog;
                    }
                }
            }
            if (UserBetShrimp != null)
            {
                foreach (BetLogBauCua betLog in UserBetShrimp)
                {
                    if (Shrimp.ContainsKey(betLog.AccountID))
                    {
                        Shrimp[betLog.AccountID].BetValue += betLog.BetValue;
                    }
                    else
                    {
                        Shrimp[betLog.AccountID] = betLog;
                    }
                }
            }
             
            return new JsonResult
            {
                //SesionId = result.SessionID,
                //CurrentStates = result.CurrentStates,
                Data = new
                {
                    SessionID = SessionID,
                    Phrase = Phrase,
                    Elapsed = Elapsed,
                    Model = _cachePvd.Exists(ModePhat)? _cachePvd.Get<List<int>>(ModePhat) : new List<int>() { 0,0,0},
                    Gourd = Gourd.Select(kvp => kvp.Value).ToList().OrderByDescending(e=>e.BetValue),
                    Crab = Crab.Select(kvp => kvp.Value).ToList().OrderByDescending(e => e.BetValue),
                    Fish = Fish.Select(kvp => kvp.Value).ToList().OrderByDescending(e => e.BetValue),
                    Chicken = Chicken.Select(kvp => kvp.Value).ToList().OrderByDescending(e => e.BetValue),
                    Shrimp = Shrimp.Select(kvp => kvp.Value).ToList().OrderByDescending(e => e.BetValue),
                    Deer = Deer.Select(kvp => kvp.Value).ToList().OrderByDescending(e => e.BetValue),

                },
                //Dice = new List<int>(result.Dice1, result.Dice2, result.Dice3)
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetModel(DataModelBauCua dataModel)
        {
            string ModePhat = CachingHandler.Instance.GeneralRedisKey("BauCua", "NewResult");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set(ModePhat,dataModel.model,1);
            return new JsonResult
            {

                Data = new
                {
                    IsOke = true
                },
            };
        }
    }
    public class DataModelBauCua
    {
        public List<int> model { set; get; }
    }
    public enum BetSideBauCua
    {
        Gourd = 1, //bau
        Crab = 2, //cua
        Fish = 3, //ca
        Chicken = 4, //ga
        Shrimp = 5, //tom
        Deer = 6 //huou
    }
    public class BetLogBauCua
    {
        public long AccountID { get; set; }
        public long BetValue { get; set; }
        public int BetSide { get; set; }
        public string Nickname { get; set; }
        public bool Bot { get; set; } = true;
        public BetLogBauCua(long accountId, long betVal, int betSide, string Nickname = "Bot", bool Bot = true)
        {
            this.AccountID = accountId;
            this.BetValue = betVal;
            this.BetSide = betSide;
            this.Nickname = Nickname;
            this.Bot = Bot;
        }
    }
}