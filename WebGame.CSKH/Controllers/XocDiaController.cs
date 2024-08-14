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
    public class XocDiaController : BaseController
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
            string keyHu = CachingHandler.Instance.GeneralRedisKey("XocDia", "Session");
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
            string ModePhat = CachingHandler.Instance.GeneralRedisKey("XocDia", "ModePhat");
            string JackpotGate = CachingHandler.Instance.GeneralRedisKey("XocDia", "JackpotGate");

            string keyHuUserBetEven = CachingHandler.Instance.GeneralRedisKey("XocDia", "UserBet:" + SessionID+":"+ BetSide.Even);
            List<BetLog> UserBetEven = new List<BetLog>();
            
            if (_cachePvd.Exists(keyHuUserBetEven))
            {
                UserBetEven =  _cachePvd.Get<List<BetLog>>(keyHuUserBetEven)?.Where(c => c.Bot == false).ToList();
            }
            string keyHuUserBetFourDown = CachingHandler.Instance.GeneralRedisKey("XocDia", "UserBet:" + SessionID + ":" + BetSide.FourDown);
            List<BetLog> UserBetFourDown = new List<BetLog>();

            if (_cachePvd.Exists(keyHuUserBetFourDown))
            {
                UserBetFourDown = _cachePvd.Get<List<BetLog>>(keyHuUserBetFourDown)?.Where(c => c.Bot == false).ToList();
            }

            string keyHuUserBetFourUp = CachingHandler.Instance.GeneralRedisKey("XocDia", "UserBet:" + SessionID + ":" + BetSide.FourUp);
            List<BetLog> UserBetFourUp = new List<BetLog>();

            if (_cachePvd.Exists(keyHuUserBetFourUp))
            {
                UserBetFourUp = _cachePvd.Get<List<BetLog>>(keyHuUserBetFourUp)?.Where(c => c.Bot == false).ToList();
            }
            string keyHuUserBetOdd = CachingHandler.Instance.GeneralRedisKey("XocDia", "UserBet:" + SessionID + ":" + BetSide.Odd);
            List<BetLog> UserBetOdd = new List<BetLog>();

            if (_cachePvd.Exists(keyHuUserBetOdd))
            {
                UserBetOdd = _cachePvd.Get<List<BetLog>>(keyHuUserBetOdd)?.Where(c => c.Bot == false).ToList();
            }
            string keyHuUserBetThreeDown = CachingHandler.Instance.GeneralRedisKey("XocDia", "UserBet:" + SessionID + ":" + BetSide.ThreeDown);
            List<BetLog> UserBetThreeDown = new List<BetLog>();

            if (_cachePvd.Exists(keyHuUserBetThreeDown))
            {
                UserBetThreeDown = _cachePvd.Get<List<BetLog>>(keyHuUserBetThreeDown)?.Where(c => c.Bot == false).ToList();
            }
            string keyHuUserBetThreeUp = CachingHandler.Instance.GeneralRedisKey("XocDia", "UserBet:" + SessionID + ":" + BetSide.ThreeUp);
            List<BetLog> UserBetThreeUp = new List<BetLog>();

            if (_cachePvd.Exists(keyHuUserBetThreeUp))
            {
                UserBetThreeUp = _cachePvd.Get<List<BetLog>>(keyHuUserBetThreeUp)?.Where(c => c.Bot == false).ToList();
            }

            Dictionary<long,BetLog> Odd = new Dictionary<long, BetLog>();
            Dictionary<long, BetLog> ThreeUp = new Dictionary<long, BetLog>();
            Dictionary<long, BetLog> ThreeDown = new Dictionary<long, BetLog>();
            Dictionary<long, BetLog> Even = new Dictionary<long, BetLog>();
            Dictionary<long, BetLog> FourUp = new Dictionary<long, BetLog>();
            Dictionary<long, BetLog> FourDown = new Dictionary<long, BetLog>();
            if (UserBetOdd!=null)
            {
                foreach (BetLog betLog in UserBetOdd)
                {
                    if (Odd.ContainsKey(betLog.AccountID))
                    {
                        Odd[betLog.AccountID].BetValue += betLog.BetValue;
                    }
                    else
                    {
                        Odd[betLog.AccountID] = betLog;
                    }
                }
            }
            if (UserBetEven != null)
            {
                foreach (BetLog betLog in UserBetEven)
                {
                    if (Even.ContainsKey(betLog.AccountID))
                    {
                        Even[betLog.AccountID].BetValue += betLog.BetValue;
                    }
                    else
                    {
                        Even[betLog.AccountID] = betLog;
                    }
                }
            }
            if (UserBetFourDown != null)
            {
                foreach (BetLog betLog in UserBetFourDown)
                {
                    if (FourDown.ContainsKey(betLog.AccountID))
                    {
                        FourDown[betLog.AccountID].BetValue += betLog.BetValue;
                    }
                    else
                    {
                        FourDown[betLog.AccountID] = betLog;
                    }
                }
            }
            if (UserBetFourUp != null)
            {
                foreach (BetLog betLog in UserBetFourUp)
                {
                    if (FourUp.ContainsKey(betLog.AccountID))
                    {
                        FourUp[betLog.AccountID].BetValue += betLog.BetValue;
                    }
                    else
                    {
                        FourUp[betLog.AccountID] = betLog;
                    }
                }
            }
            if (UserBetThreeDown != null)
            {
                foreach (BetLog betLog in UserBetThreeDown)
                {
                    if (ThreeDown.ContainsKey(betLog.AccountID))
                    {
                        ThreeDown[betLog.AccountID].BetValue += betLog.BetValue;
                    }
                    else
                    {
                        ThreeDown[betLog.AccountID] = betLog;
                    }
                }
            }
            if (UserBetThreeUp != null)
            {
                foreach (BetLog betLog in UserBetThreeUp)
                {
                    if (ThreeUp.ContainsKey(betLog.AccountID))
                    {
                        ThreeUp[betLog.AccountID].BetValue += betLog.BetValue;
                    }
                    else
                    {
                        ThreeUp[betLog.AccountID] = betLog;
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
                    Model = _cachePvd.Exists(ModePhat)? _cachePvd.Get<int>(ModePhat) : -1,
                    Jackpot = _cachePvd.Exists(JackpotGate) ? _cachePvd.Get<int>(JackpotGate) : -1,
                    Odd = Odd.Select(kvp => kvp.Value).ToList().OrderByDescending(e=>e.BetValue),
                    ThreeUp = ThreeUp.Select(kvp => kvp.Value).ToList().OrderByDescending(e => e.BetValue),
                    ThreeDown = ThreeDown.Select(kvp => kvp.Value).ToList().OrderByDescending(e => e.BetValue),
                    Even = Even.Select(kvp => kvp.Value).ToList().OrderByDescending(e => e.BetValue),
                    FourUp = FourUp.Select(kvp => kvp.Value).ToList().OrderByDescending(e => e.BetValue),
                    FourDown = FourDown.Select(kvp => kvp.Value).ToList().OrderByDescending(e => e.BetValue),

                },
                //Dice = new List<int>(result.Dice1, result.Dice2, result.Dice3)
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetJackpot(DataJackpot dataJackpot)
        {
            string JackpotGate = CachingHandler.Instance.GeneralRedisKey("XocDia", "JackpotGate");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set(JackpotGate, dataJackpot.jackpot, 1);
            return new JsonResult
            {
                Data = new
                {
                    IsOke = true
                },
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetModel(DataModel dataModel)
        {
            string ModePhat = CachingHandler.Instance.GeneralRedisKey("XocDia", "ModePhat");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set(ModePhat,dataModel.model, 1);
            return new JsonResult
            {
                Data = new
                {
                    IsOke = true
                },
            };
        }
    }
    public class DataModel
    {
        public int model { set; get; }
    }
    public class DataJackpot
    {
        public int jackpot { set; get; }
    }
    public enum BetSide
    {
        None = -1,
        Odd = 1,
        ThreeUp = 2,
        ThreeDown = 3,
        Even = 4,
        FourUp = 5,
        FourDown = 6
    }
    public class BetLog
    {
        public long AccountID { get;  set; }
        public long BetValue { get;  set; }
        public int BetSide { get;  set; }
        public string Nickname { get;  set; }
        public bool Bot { get;  set; } = true;
        public BetLog(long accountId, long betVal, int betSide, string Nickname = "Bot", bool Bot = true)
        {
            this.AccountID = accountId;
            this.BetValue = betVal;
            this.BetSide = betSide;
            this.Nickname = Nickname;
            this.Bot = Bot;
        }
    }
}