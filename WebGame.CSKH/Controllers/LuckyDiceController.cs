using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.LuckyDice;
using MsWebGame.CSKH.Utils;
using MsWebGame.RedisCache.Cache;
using MsWebGame.CSKH.Models.Accounts;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class LuckyDiceController : BaseController
    {
        // GET: LuckyDice
        public ActionResult Index()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Event()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult RaceTop()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult AddOrUpdateEvent()
        {
            EventModel model = new EventModel();
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult ReportEvent()
        {
            ViewBag.PartialEvents = ComboboxEvents();
            ViewBag.PartialAwards = ComboboxAwards();
            ReportEventModel model = new ReportEventModel();
            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Now;
            model.RaceTopID = -1;
            model.EventID = 0;
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdateEvent(EventModel model)
        {
            EventModel eventModel = LuckyDiceDAO.Instance.GetEventInfo(model.EventID);
            return View("AddOrUpdateEvent", eventModel);
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetEventList(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            var list = LuckyDiceDAO.Instance.GetEventList();
            var gridModel = new GridModel<EventModel>
            {
                Data = list,
                Total = list.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult SaveEvent(EventModel model, GridCommand command)
        {
            int response = LuckyDiceDAO.Instance.SaveEvent(model);
            if (response < 0)
            {
                string msg = MessageConvetor.MsgLuckyDice.GetEventMessage(response);
                ErrorNotification(msg);
                return View("AddOrUpdateEvent", model);
            }
            return RedirectToAction("Event", "LuckyDice");
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult DeleteEvent(EventModel model, GridCommand command)
        {
            int response = LuckyDiceDAO.Instance.DeleteEvent(model);
            if (response < 0)
            {
                ErrorNotification(Message.DeleteFail);
            }else
            {
                SuccessNotification(Message.DeleteSuccess);
            }
            return RedirectToAction("Event");
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult AddOrUpdateRaceTop()
        {
            RaceTopModel model = new RaceTopModel();
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Quy()
        {
            ViewBag.Logs = LuckyDiceDAO.Instance.GetSoiCau(15);
            return View();
        }
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdateRaceTop(RaceTopModel model)
        {
            RaceTopModel eventModel = LuckyDiceDAO.Instance.GetRaceTopInfo(model.RaceTopID);
            return View("AddOrUpdateRaceTop", eventModel);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult SaveRaceTop(RaceTopModel model, GridCommand command)
        {
            int response = LuckyDiceDAO.Instance.SaveRaceTop(model);
            if (response < 0)
            {
                string msg = MessageConvetor.MsgLuckyDice.GetEventMessage(response);
                ErrorNotification(msg);
                return View("AddOrUpdateRaceTop", model);
            }
            return RedirectToAction("RaceTop", "LuckyDice");
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult DeleteRaceTop(RaceTopModel model, GridCommand command)
        {
            int response = LuckyDiceDAO.Instance.DeleteRaceTop(model);
            if (response < 0)
            {
                ErrorNotification(Message.DeleteFail);
            }
            else
            {
                SuccessNotification(Message.DeleteSuccess);
            }
            return RedirectToAction("RaceTop");
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetRaceTopList(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            var list = LuckyDiceDAO.Instance.GetRaceTopList();
            var gridModel = new GridModel<RaceTopModel>
            {
                Data = list,
                Total = list.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetEventAwardList(ReportEventModel model, GridCommand command)
        {
            int totalRecord = 0;
            var list = LuckyDiceDAO.Instance.GetEventAwardList(model, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var gridModel = new GridModel<ReportEventModel>
            {
                Data = list,
                Total = totalRecord
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        private List<EventModel> ComboboxEvents()
        {
            var list = new List<EventModel>();
            var eventList = LuckyDiceDAO.Instance.GetEventList();
            list.AddRange(eventList);
            return list;
        }

        private List<RaceTopModel> ComboboxAwards()
        {
            var list = new List<RaceTopModel>();
            list.Add(new RaceTopModel
            {
                RaceTopID = 0,
                Description = "Triệu hồi"
            });
            var eventList = LuckyDiceDAO.Instance.GetRaceTopList();
            list.AddRange(eventList);

            return list;
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult InitSession(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keySession = "tx.session:info";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            RedisSession result = _cachePvd.Get<RedisSession>(keySession);
            if (result == null)
            {
                result = new RedisSession();
            }
            long TotalBetTai = 0, TotalBetXiu = 0, TotalTai = 0, TotalXiu = 0;
            string key = Helpers.LuckyDice.Helper.GenerateKey(result.SessionID, (int)Helpers.LuckyDice.KeyType.TotalBet, 0, (int)Helpers.LuckyDice.BetSide.Tai);

            if (_cachePvd.Exists(key))
                TotalBetTai = _cachePvd.Get<long>(key);
            key = Helpers.LuckyDice.Helper.GenerateKey(result.SessionID, (int)Helpers.LuckyDice.KeyType.TotalBet, 0, (int)Helpers.LuckyDice.BetSide.Xiu);
            if (_cachePvd.Exists(key))
                TotalBetXiu = _cachePvd.Get<long>(key);
            key = Helpers.LuckyDice.Helper.GenerateKey(result.SessionID, (int)Helpers.LuckyDice.KeyType.Turn, 0, (int)Helpers.LuckyDice.BetSide.Tai);
            if (_cachePvd.Exists(key))
                TotalTai = _cachePvd.Get<long>(key);
            key = Helpers.LuckyDice.Helper.GenerateKey(result.SessionID, (int)Helpers.LuckyDice.KeyType.Turn, 0, (int)Helpers.LuckyDice.BetSide.Xiu);
            if (_cachePvd.Exists(key))
                TotalXiu = _cachePvd.Get<long>(key);

            List<UserBetBalance> BetBalancesTai = new List<UserBetBalance>();
            string keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", (result.CurrentState == 0 ? "UserBetBalance." + 0 + ".user" : "UserBetBalance_Tmp." + 0));

            if (_cachePvd.Exists(keyHu))
            {
                BetBalancesTai = _cachePvd.Get<List<UserBetBalance>>(keyHu);
            }

            List<UserBetBalance> BetBalancesXiu = new List<UserBetBalance>();
            keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", (result.CurrentState == 0 ? "UserBetBalance." + 1 + ".user" : "UserBetBalance_Tmp." + 1));

            if (_cachePvd.Exists(keyHu))
            {
                BetBalancesXiu = _cachePvd.Get<List<UserBetBalance>>(keyHu);
            }
            keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "Mode");

            List<UserBetBalance> ListBetBalancesTai = BetBalancesTai.FindAll(e => e.AccountID > 0);
            List<UserBetBalance> ListBetBalancesXiu = BetBalancesXiu.FindAll(e => e.AccountID > 0);

            List<UserBetBalance> DataBetBalancesTai = new List<UserBetBalance>();
            foreach (var val in ListBetBalancesTai)
            {

                UserBetBalance userBetBalance = DataBetBalancesTai.Find(x => x.AccountID == val.AccountID);

                if (result.SessionID != val.SessionID)
                {
                    continue;
                }
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(8, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 70);
                    }

                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;
                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);
                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }
                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);
                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesTai.Add(val);
                }
            }
            List<UserBetBalance> DataBetBalancesXiu = new List<UserBetBalance>();
            foreach (var val in ListBetBalancesXiu)
            {
                UserBetBalance userBetBalance = DataBetBalancesXiu.Find(x => x.AccountID == val.AccountID);

                if (result.SessionID != val.SessionID)
                {
                    continue;
                }
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(8, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 60);
                    }
                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);

                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }

                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);

                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesXiu.Add(val);
                }
            }

            return new JsonResult
            {
                Data = new
                {
                    SessionID = result.SessionID,
                    BetBalancesTai = DataBetBalancesTai.OrderByDescending(p => p.Amount).ToList(),
                    BetBalancesXiu = DataBetBalancesXiu.OrderByDescending(p => p.Amount).ToList(),
                    Dices = new List<int>() { result.Dice1, result.Dice2, result.Dice3 },
                    Ellapsed = result.Ellapsed,
                    TotalBetTai = TotalBetTai,
                    TotalBetXiu = TotalBetXiu,
                    TotalTai = TotalTai,
                    TotalXiu = TotalXiu,
                    CurrentState = result.CurrentState,
                    Model = _cachePvd.Get<int>(keyHu),
                    Logs = result.CurrentState == MsWebGame.CSKH.Helpers.LuckyDice.GameState.ShowResult ? LuckyDiceDAO.Instance.GetSoiCau(15) : new List<SoiCau>(),
                    JkpNickname = _cachePvd.Exists("tx.session:info:autoJackpot:nickname") ? _cachePvd.Get<string>("tx.session:info:autoJackpot:nickname") : ""
                },
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetModel(LuckyDiceModel input)
        {
            //lay danh sách chăm sóc khách hàng
            string keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "Mode");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            int _Model = (input.Model == 1 ? 0 : (input.Model == 2 ? 1 : -1));
            _cachePvd.Set(keyHu, _Model);

            return new JsonResult
            {
                Data = new
                {
                    IsOke = _cachePvd.Get<int>(keyHu) == _Model
                },
            };
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult JackpotSet(DataPostSetJackpot data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("tx.session:info:autoJackpot:nickname", data.Nickname);
            return new JsonResult
            {
                Data = new
                {
                    Status = 1
                }
            };
        }
    }
}