using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Models.Accounts;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.SicboChanLeLuckyDice;
using TraditionGame.Utilities;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using MsWebGame.RedisCache.Cache;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class RongHoController : BaseController
    {
        // GET: Md5LuckyDice
        public ActionResult Index()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult InitSession(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keySession = "rongho:session.info";
            string keyHu = CachingHandler.Instance.GeneralRedisKey("RongHo", "Mode");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            RedisCacheProvider _cachePvd2 = new RedisCacheProvider();
            RongHoData result = _cachePvd.Get<RongHoData>(keySession);
            var result2 = _cachePvd2.Get<int>(keyHu);

            if (result == null)
            {
                result = new RongHoData();
            }

            var Dragon = new List<RongHoBetData>();

            foreach (var x in result.Dragon)
            {
                var accountInfo = AccountProfileDAO.Instance.GetAccountInfor(x.Key, null, null, 1);

                if (accountInfo != null)
                {
                    Dragon.Add(new RongHoBetData
                    {
                        BetValue = x.Value,
                        Nickname = accountInfo.AccountName,
                        Balance = string.Format("{0:#,###,###}", accountInfo.Balance)
                });
                }
            }

            var Tie = new List<RongHoBetData>();

            foreach (var x in result.Tie)
            {
                var accountInfo = AccountProfileDAO.Instance.GetAccountInfor(x.Key, null, null, 1);

                if (accountInfo != null)
                {
                    Tie.Add(new RongHoBetData
                    {
                        BetValue = x.Value,
                        Nickname = accountInfo.AccountName,
                        Balance = string.Format("{0:#,###,###}", accountInfo.Balance)
                    });
                }
            }

            var Tiger = new List<RongHoBetData>();

            foreach (var x in result.Tiger)
            {
                var accountInfo = AccountProfileDAO.Instance.GetAccountInfor(x.Key, null, null, 1);

                if (accountInfo != null)
                {
                    Tiger.Add(new RongHoBetData
                    {
                        BetValue = x.Value,
                        Nickname = accountInfo.AccountName,
                        Balance = string.Format("{0:#,###,###}", accountInfo.Balance)
                    });
                }
            }

            return new JsonResult
            {
                Data = new
                {
                    Phrase = result.Phrase,
                    SessionID = result.SessionID,
                    Elapsed = result.Elapsed,
                    Dragon = Dragon,
                    Tie = Tie,
                    Tiger = Tiger,
                    Model = result2
                },
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetModel(RongHoModel input)
        {
            NLogManager.Debug(JsonConvert.SerializeObject(input));
            string keyHu = CachingHandler.Instance.GeneralRedisKey("RongHo", "Mode");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set(keyHu, input.Model);
            NLogManager.Debug(JsonConvert.SerializeObject(input.Model));
            string keySession = "rongho:session.info";

            return new JsonResult
            {
                Data = new
                {
                    IsOke = true
                },
            };
        }
    }
    public class RongHoModel
    {
        public int Model
        {
            get;
            set;
        }
    }
    public class RongHoData
    {
        public string Phrase;
        public long SessionID;
        public int Elapsed;
        public Dictionary<long, long> Dragon = new Dictionary<long, long>();
        public Dictionary<long, long> Tie = new Dictionary<long, long>();
        public Dictionary<long, long> Tiger = new Dictionary<long, long>();
        public int Model;
    }
    public class RongHoBetData
    {
        public string Nickname;
        public float BetValue;
        public string Balance;
    }
}