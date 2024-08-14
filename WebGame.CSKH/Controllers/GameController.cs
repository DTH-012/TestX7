using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using TraditionGame.Utilities;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.Games;
using MsWebGame.CSKH.Models.Redis.Cowboys;
using MsWebGame.RedisCache.Cache;

namespace MsWebGame.CSKH.Controllers
{
    public class GameController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost]
        public ActionResult PopupBankView(Models.Param.ParBankGame data)
        {
            string GameName = "";
            string Type = "RoomFunds";
            string RoomId = data.RoomId.ToString();
            string Action = "GameFundModel";
            if (data.GameId == 19)
            {
                GameName = "Baccarat";
                RoomId = "";
                Action = "Number";
            }
            else if (data.GameId == 20)
            {
                GameName = "BauCua";
                RoomId = "";
                Action = "Number";
            }
            else if (data.GameId == 12)
            {
                GameName = "BlockBuster";
            }else if (data.GameId == 3)
            {
                GameName = "Cowboy";
            }
            else if (data.GameId == 4)
            {
                GameName = "Egypt";
            }
            else if (data.GameId == 1)
            {
                GameName = "Islands";
            }
            else if (data.GameId == 7)
            {
                GameName = "KingStar";
            }
           
            else if (data.GameId == 14)
            {
                GameName = "RongHo";
                RoomId = "";
                Action = "Number";
            } 
            else if (data.GameId == 63)
            {
                GameName = "Sedie";
                RoomId = "";
                Action = "Number";
            }
            else if (data.GameId == 15)
            {
                GameName = "Songoku";
            } 
            else if (data.GameId == 2)
            {
                GameName = "Tayduky";
            }
            else if (data.GameId == 11)
            {
                GameName = "Xpoker";
            }
            else if (data.GameId == 8)
            {
                GameName = "TaiXiu";
                Type = "FundSets";
                Action = "BotFundSets";
                RoomId = "";
            }
            string keyCa = CachingHandler.Instance.GeneralRedisKey(GameName, Type + RoomId);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            long PrizeFund = 0;
            long JackpotFund = 0;
            ViewBag.Key = keyCa;
            ViewBag.RoomId = RoomId;
            if (_cachePvd.Exists(keyCa))
            {
                if (Action == "GameFundModel")
                {
                    GameFundModel gameFund = _cachePvd.Get<GameFundModel>(keyCa);

                    PrizeFund = gameFund.PrizeFund.Value;
                    JackpotFund = gameFund.JackpotFund.Value;

                    ViewBag.PrizeFund = PrizeFund;
                    ViewBag.JackpotFund = JackpotFund;
                }
                else if (Action == "Number")
                {
                    ViewBag.TotalBank = _cachePvd.Get<long>(keyCa); ;
                }
                else if (Action == "BotFundSets")
                {
                    BotFundSets gameFund = _cachePvd.Get<BotFundSets>(keyCa);

                    ViewBag.PrizeValue = gameFund.PrizeValue;
                    ViewBag.Balance = gameFund.Balance;
                    ViewBag.TotalAddBalance = gameFund.TotalAddBalance;
                }
            }
            ViewBag.Action = Action;
            return PartialView();
        }
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost]
        public ActionResult PopupBank(Models.Param.ParBankGameForm data)
        {
            string GameName = "";
            string Type = "RoomFunds";
            string RoomId = data.RoomId.ToString();
            string Action = "GameFundModel";
            if (data.GameId == 19)
            {
                GameName = "Baccarat";
                RoomId = "";
                Action = "Number";
            }
            else if (data.GameId == 20)
            {
                GameName = "BauCua";
                RoomId = "";
                Action = "Number";
            }
            else if (data.GameId == 12)
            {
                GameName = "BlockBuster";
            }
            else if (data.GameId == 3)
            {
                GameName = "Cowboy";
            }
            else if (data.GameId == 4)
            {
                GameName = "Egypt";
            }
            else if (data.GameId == 1)
            {
                GameName = "Islands";
            }
            else if (data.GameId == 7)
            {
                GameName = "KingStar";
            }

            else if (data.GameId == 14)
            {
                GameName = "RongHo";
                RoomId = "";
                Action = "Number";
            }
            else if (data.GameId == 63)
            {
                GameName = "Sedie";
                RoomId = "";
                Action = "Number";
            }
            else if (data.GameId == 15)
            {
                GameName = "Songoku";
            }
            else if (data.GameId == 2)
            {
                GameName = "Tayduky";
            }
            else if (data.GameId == 11)
            {
                GameName = "Xpoker";
            }
            else if (data.GameId == 8)
            {
                GameName = "TaiXiu";
                Type = "FundSets";
                Action = "BotFundSets";
                RoomId = "";
            }
            string keyCa = CachingHandler.Instance.GeneralRedisKey(GameName, Type + RoomId);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            object rs = null;
            if (_cachePvd.Exists(keyCa))
            {
                if (Action == "GameFundModel")
                {
                    GameFundModel gameFund = _cachePvd.Get<GameFundModel>(keyCa);
                    _cachePvd.Set(keyCa, new GameFundModel() { RoomID = data.RoomId, PrizeFund = data.PrizeFund,JackpotFund = data.JackpotFund  });
                }
                else if (Action == "Number")
                {
                    _cachePvd.Set(keyCa, data.TotalBank);
                }
                else if (Action == "BotFundSets")
                {
                    _cachePvd.Set(keyCa, new BotFundSets() {
                        PrizeValue = data.PrizeValue, 
                        Balance = data.Balance,
                        TotalAddBalance = data.TotalAddBalance
                    });
                }
            }
            return new JsonResult
            {
                Data = new
                {
                     
                }
            };
        }
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult GameFund()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GameFundList(GridCommand command)
        {
            try
            {

                int totalRecord = 50;
                var list = GameDAO.Instance.GameFundsList();

                NLogManager.LogError(list.Count.ToString());

                list.RemoveAll(c =>
                    c.GameID == 3 || c.GameID == 4 || c.GameID == 15 || c.GameID == 2 || c.GameID == 1 ||
                    c.GameID == 12 || c.GameID == 7 || c.GameID == 11 || c.GameID == 19 || c.GameID == 20 || c.GameID == 14 || c.GameID == 63);
                list.Add(GetCowboys(1));
                list.Add(GetCowboys(2));
                list.Add(GetCowboys(3));
                list.Add(GetCowboys(4));

                list.Add(GetEgypt(1));
                list.Add(GetEgypt(2));
                list.Add(GetEgypt(3));
                list.Add(GetEgypt(4));

                list.Add(GetSonGoKu(1));
                list.Add(GetSonGoKu(2));
                list.Add(GetSonGoKu(3));
                list.Add(GetSonGoKu(4));

                list.Add(GetTayduky(1));
                list.Add(GetTayduky(2));
                list.Add(GetTayduky(3));
                list.Add(GetTayduky(4));

                list.Add(GetThuyCung(1));
                list.Add(GetThuyCung(2));
                list.Add(GetThuyCung(3));
                list.Add(GetThuyCung(4));

                list.Add(GetBlockBuster(1));
                list.Add(GetBlockBuster(2));
                list.Add(GetBlockBuster(3));
                list.Add(GetBlockBuster(4));
                list.Add(GetBlockBuster(5));

                list.Add(GetCandy(1));
                list.Add(GetCandy(2));
                list.Add(GetCandy(3));
                list.Add(GetCandy(4));

                list.Add(GetMiniPoker(1));
                list.Add(GetMiniPoker(2));
                list.Add(GetMiniPoker(3));

                list.Add(GetBaccarat());
                list.Add(GetLocThu());
                list.Add(GetRongHo());
                list.Add(GetXocDia());
                list.Add(GetTaiXiu());
                list = list.OrderBy(c => c.GameID).ToList();
                var gridModel = new GridModel<GameFundModel>
                {
                    Data = list.Select(x =>
                    {
                        var m = new GameFundModel();
                        m = Mapper.Map<GameFundModel>(x);
                        return m;
                    }),
                    Total = totalRecord
                };
                return new JsonResult { Data = gridModel };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult GameIndex()
        {
            GameIndexListModel model = new GameIndexListModel();
            var now = DateTime.Now;
            DateTime startTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            model.searchDate = startTime;
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GameIndexList(GridCommand command, DateTime? searchDate)
        {
            if (searchDate.HasValue)
            {
                var date = searchDate.Value;
                DateTime startTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                searchDate = startTime;
            }
            else
            {
                var now = DateTime.Now;
                DateTime startTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                searchDate = startTime;
            }
            int totalRecord = 50;
            var list = GameDAO.Instance.GetGameIndexList(searchDate);
            if (list == null)
                list = new List<Database.DTO.GameIndex>();

            var gridModel = new GridModel<GameIndexModel>
            {
                Data = list.Select(x =>
                {
                    var m = new GameIndexModel();
                    m = Mapper.Map<GameIndexModel>(x);
                    return m;
                }),
                Total = totalRecord
            };
            return new JsonResult { Data = gridModel };
        }

        GameFunds GetCowboys(int romId)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("Cowboy", "RoomFunds" + romId);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
            GameFunds roms = new GameFunds();
            roms.GameID = 3;
            roms.GameName = "Cao bồi";
            roms.RoomID = romId;
            roms.PrizeFund = room?.PrizeFund;
            roms.JackpotFund = room?.JackpotFund;
            return roms;
        }

        GameFunds GetEgypt(int romId)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("Egypt", "RoomFunds" + romId);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            MsWebGame.CSKH.Models.Redis.Egypt.RoomFunds room = _cachePvd.Get<MsWebGame.CSKH.Models.Redis.Egypt.RoomFunds>(keyHu);
            GameFunds roms = new GameFunds();
            roms.GameID = 4;
            roms.GameName = "Ai cập";
            roms.RoomID = romId;
            roms.PrizeFund = room?.PrizeFund;
            roms.JackpotFund = room?.JackpotFund;
            return roms;
        }

        GameFunds GetSonGoKu(int romId)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("Songoku", "RoomFunds" + romId);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            MsWebGame.CSKH.Models.Redis.Songoku.RoomFunds room = _cachePvd.Get<MsWebGame.CSKH.Models.Redis.Songoku.RoomFunds>(keyHu);
            GameFunds roms = new GameFunds();
            roms.GameID = 15;
            roms.GameName = "Songoku";
            roms.RoomID = romId;
            roms.PrizeFund = room?.PrizeFund;
            roms.JackpotFund = room?.JackpotFund;
            return roms;
        }

        GameFunds GetTayduky(int romId)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("Tayduky", "RoomFunds" + romId);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            MsWebGame.CSKH.Models.Redis.Tayduky.RoomFunds room = _cachePvd.Get<MsWebGame.CSKH.Models.Redis.Tayduky.RoomFunds>(keyHu);
            GameFunds roms = new GameFunds();
            roms.GameID = 2;
            roms.GameName = "Tam Quốc";
            roms.RoomID = romId;
            roms.PrizeFund = room?.PrizeFund;
            roms.JackpotFund = room?.JackpotFund;
            return roms;
        }

        GameFunds GetThuyCung(int romId)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("Islands", "RoomFunds" + romId);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            MsWebGame.CSKH.Models.Redis.ThuyCung.RoomFunds room = _cachePvd.Get<MsWebGame.CSKH.Models.Redis.ThuyCung.RoomFunds>(keyHu);
            GameFunds roms = new GameFunds();
            roms.GameID = 1;
            roms.GameName = "Thủy Cung";
            roms.RoomID = romId;
            roms.PrizeFund = room?.PrizeFund;
            roms.JackpotFund = room?.JackpotFund;
            return roms;
        }

        GameFunds GetBlockBuster(int romId)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("BlockBuster", "RoomFunds" + romId);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            MsWebGame.CSKH.Models.Redis.Tayduky.RoomFunds room = _cachePvd.Get<MsWebGame.CSKH.Models.Redis.Tayduky.RoomFunds>(keyHu);
            GameFunds roms = new GameFunds();
            roms.GameID = 12;
            roms.GameName = "Siêu Xe";
            roms.RoomID = romId;
            roms.PrizeFund = room?.PrizeFund;
            roms.JackpotFund = room?.JackpotFund;
            return roms;
        }

        GameFunds GetCandy(int romId)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("KingStar", "RoomFunds" + romId);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            MsWebGame.CSKH.Models.Redis.Tayduky.RoomFunds room = _cachePvd.Get<MsWebGame.CSKH.Models.Redis.Tayduky.RoomFunds>(keyHu);
            GameFunds roms = new GameFunds();
            roms.GameID = 7;
            roms.GameName = "Candy";
            roms.RoomID = romId;
            roms.PrizeFund = room?.PrizeFund;
            roms.JackpotFund = room?.JackpotFund;
            return roms;
        }

        GameFunds GetMiniPoker(int romId)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("Xpoker", "RoomFunds" + romId);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            MsWebGame.CSKH.Models.Redis.Tayduky.RoomFunds room = _cachePvd.Get<MsWebGame.CSKH.Models.Redis.Tayduky.RoomFunds>(keyHu);
            GameFunds roms = new GameFunds();
            roms.GameID = 11;
            roms.GameName = "MiniPoker";
            roms.RoomID = romId;
            roms.PrizeFund = room?.PrizeFund;
            roms.JackpotFund = room?.JackpotFund;
            return roms;
        }

        GameFunds GetBaccarat()
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("Baccarat", "RoomFunds");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            long prizeFund = 0;
            if (_cachePvd.Exists(keyHu))
            {
                prizeFund = _cachePvd.Get<long>(keyHu);

            }
            GameFunds roms = new GameFunds();
            roms.GameID = 19;
            roms.GameName = "Baccarat";
            roms.RoomID = 0;
            roms.PrizeFund = prizeFund;
            roms.JackpotFund = 0;
            return roms;
        }

        GameFunds GetLocThu()
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("BauCua", "RoomFunds");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            long prizeFund = 0;
            if (_cachePvd.Exists(keyHu))
            {
                prizeFund = _cachePvd.Get<long>(keyHu);

            }
            GameFunds roms = new GameFunds();
            roms.GameID = 20;
            roms.GameName = "Lộc Thú";
            roms.RoomID = 0;
            roms.PrizeFund = prizeFund;
            roms.JackpotFund = 0;
            return roms;
        }

        GameFunds GetRongHo()
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("RongHo", "RoomFunds");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            long prizeFund = 0;
            if (_cachePvd.Exists(keyHu))
            {
                prizeFund = _cachePvd.Get<long>(keyHu);

            }
            GameFunds roms = new GameFunds();
            roms.GameID = 14;
            roms.GameName = "Rồng Hổ";
            roms.RoomID = 0;
            roms.PrizeFund = prizeFund;
            roms.JackpotFund = 0;
            return roms;
        }

        GameFunds GetXocDia()
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "RoomFunds");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            long prizeFund = 0;
            if (_cachePvd.Exists(keyHu))
            {
                prizeFund = _cachePvd.Get<long>(keyHu);

            }
            GameFunds roms = new GameFunds();
            roms.GameID = 63;
            roms.GameName = "Xóc Đĩa";
            roms.RoomID = 0;
            roms.PrizeFund = prizeFund;
            roms.JackpotFund = 0;
            return roms;
        }
        GameFunds GetTaiXiu()
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "FundSets");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            BotFundSets room = _cachePvd.Get<BotFundSets>(keyHu);
            GameFunds roms = new GameFunds();
            roms.GameID = 8;
            roms.GameName = "Tài xỉu Mini";
            roms.RoomID = 0;
            roms.PrizeFund = room?.Balance;
            roms.JackpotFund = room?.PrizeValue;
           
            return roms;
        }
    }
}