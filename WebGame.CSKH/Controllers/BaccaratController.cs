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
    public class BaccaratController : BaseController
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
            string keySession = "baccarat:session.info";
            string keyHu = CachingHandler.Instance.GeneralRedisKey("Baccarat", "Mode");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            RedisCacheProvider _cachePvd2 = new RedisCacheProvider();
            BaccaratData result = _cachePvd.Get<BaccaratData>(keySession);
            var result2 = _cachePvd2.Get<int>(keyHu);

            if (result == null)
            {
                result = new BaccaratData();
            }

            var Player = new List<BaccaratBetData>();

            foreach (var x in result.Player)
            {
                var accountInfo = AccountProfileDAO.Instance.GetAccountInfor(x.Key, null, null, 1);

                if (accountInfo != null)
                {
                    Player.Add(new BaccaratBetData
                    {
                        BetValue = x.Value,
                        Nickname = accountInfo.AccountName
                    });
                }
            }

            var Tie = new List<BaccaratBetData>();

            foreach (var x in result.Tie)
            {
                var accountInfo = AccountProfileDAO.Instance.GetAccountInfor(x.Key, null, null, 1);

                if (accountInfo != null)
                {
                    Tie.Add(new BaccaratBetData
                    {
                        BetValue = x.Value,
                        Nickname = accountInfo.AccountName
                    });
                }
            }

            var Banker = new List<BaccaratBetData>();

            foreach (var x in result.Banker)
            {
                var accountInfo = AccountProfileDAO.Instance.GetAccountInfor(x.Key, null, null, 1);

                if (accountInfo != null)
                {
                    Banker.Add(new BaccaratBetData
                    {
                        BetValue = x.Value,
                        Nickname = accountInfo.AccountName
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
                    Player = Player,
                    Tie = Tie,
                    Banker = Banker,
                    Model = result2
                },
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetModel(BaccaratModel input)
        {
            NLogManager.Debug(JsonConvert.SerializeObject(input));
            string keyHu = CachingHandler.Instance.GeneralRedisKey("Baccarat", "Mode");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set(keyHu, input.Model);
            NLogManager.Debug(JsonConvert.SerializeObject(input.Model));
            string keySession = "baccarat:session.info";

            return new JsonResult
            {
                Data = new
                {
                    IsOke = true
                },
            };
        }
    }
    public class BaccaratModel
    {
        public int Model
        {
            get;
            set;
        }
    }
    public class BaccaratData
    {
        public string Phrase;
        public long SessionID;
        public int Elapsed;
        public Dictionary<long, long> Player = new Dictionary<long, long>();
        public Dictionary<long, long> Tie = new Dictionary<long, long>();
        public Dictionary<long, long> Banker = new Dictionary<long, long>();
        public int Model;
    }
    public class BaccaratBetData
    {
        public string Nickname;
        public float BetValue;
    }
}