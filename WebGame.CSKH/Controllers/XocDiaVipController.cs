﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.XocDiaVip;
using MsWebGame.CSKH.Utils;
using MsWebGame.RedisCache.Cache;
using MsWebGame.CSKH.Models.Accounts;
using MsTraditionGame.Utilities.Log;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class XocDiaVipController : BaseController
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
            //EventModel model = new EventModel();
            EventModel eventModel = XocDiaVipDAO.Instance.GetEventInfo(model.EventID);
            return View("AddOrUpdateEvent", eventModel);
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetEventList(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng

            var list = XocDiaVipDAO.Instance.GetEventList();
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
            int response = XocDiaVipDAO.Instance.SaveEvent(model);
            if (response < 0)
            {
                string msg = MessageConvetor.MsgLuckyDice.GetEventMessage(response);
                ErrorNotification(msg);
                return View("AddOrUpdateEvent", model);
            }
            return RedirectToAction("Event", "XocDiaVip");
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult DeleteEvent(EventModel model, GridCommand command)
        {
            int response = XocDiaVipDAO.Instance.DeleteEvent(model);
            if (response < 0)
            {
                ErrorNotification(Message.DeleteFail);
            }
            else
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
        public ActionResult QuyXocDiaVip()
        {
            ViewBag.Logs = XocDiaVipDAO.Instance.GetSoiCau(15);
            return View();
        }
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdateRaceTop(RaceTopModel model)
        {
            //EventModel model = new EventModel();
            RaceTopModel eventModel = XocDiaVipDAO.Instance.GetRaceTopInfo(model.RaceTopID);
            return View("AddOrUpdateRaceTop", eventModel);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult SaveRaceTop(RaceTopModel model, GridCommand command)
        {
            int response = XocDiaVipDAO.Instance.SaveRaceTop(model);
            if (response < 0)
            {
                string msg = MessageConvetor.MsgLuckyDice.GetEventMessage(response);
                ErrorNotification(msg);
                return View("AddOrUpdateRaceTop", model);
            }
            return RedirectToAction("RaceTop", "XocDiaVip");
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult DeleteRaceTop(RaceTopModel model, GridCommand command)
        {
            int response = XocDiaVipDAO.Instance.DeleteRaceTop(model);
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
            var list = XocDiaVipDAO.Instance.GetRaceTopList();
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
            var list = XocDiaVipDAO.Instance.GetEventAwardList(model, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
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
            var eventList = XocDiaVipDAO.Instance.GetEventList();
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
            var eventList = XocDiaVipDAO.Instance.GetRaceTopList();
            list.AddRange(eventList);

            return list;
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult InitSession(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keySession = "xocdiavip.session:info";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            XocDiaVipRedisSession result = _cachePvd.Get<XocDiaVipRedisSession>(keySession);
            if (result == null)
            {
                result = new XocDiaVipRedisSession();
            }

            List<UserBetBalance> BetBalancesTai = new List<UserBetBalance>();
            string keyHu = CachingHandler.Instance.GeneralRedisKey("XocDiaVip", "UserBetBalance." + 4);

            if (_cachePvd.Exists(keyHu))
            {
                BetBalancesTai = _cachePvd.Get<List<UserBetBalance>>(keyHu);
            }

            List<UserBetBalance> BetBalancesXiu = new List<UserBetBalance>();
            keyHu = CachingHandler.Instance.GeneralRedisKey("XocDiaVip", "UserBetBalance." + 1);

            if (_cachePvd.Exists(keyHu))
            {
                BetBalancesXiu = _cachePvd.Get<List<UserBetBalance>>(keyHu);
            }
            keyHu = CachingHandler.Instance.GeneralRedisKey("XocDiaVip", "Mode");

            List<UserBetBalance> ListBetBalancesTai = BetBalancesTai.FindAll(e => e.AccountID > 0);
            List<UserBetBalance> ListBetBalancesXiu = BetBalancesXiu.FindAll(e => e.AccountID > 0);
            List<UserBetBalance> DataBetBalancesTai = new List<UserBetBalance>();
            foreach (var val in ListBetBalancesTai)
            {
                UserBetBalance userBetBalance = DataBetBalancesTai.Find(x => x.AccountID == val.AccountID);

                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {

                    DataBetBalancesTai.Add(val);
                }
            }
            List<UserBetBalance> DataBetBalancesXiu = new List<UserBetBalance>();
            foreach (var val in ListBetBalancesXiu)
            {
                UserBetBalance userBetBalance = DataBetBalancesXiu.Find(x => x.AccountID == val.AccountID);
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {


                    DataBetBalancesXiu.Add(val);
                }
            }

            return new JsonResult
            {
                Data = new
                {
                    SessionID = result.SessionID,
                    BetBalancesTai = DataBetBalancesTai,
                    BetBalancesXiu = DataBetBalancesXiu,
                    Ellapsed = result.Ellapsed,
                    Model = _cachePvd.Get<int>(keyHu),
                },
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetModel(XocDiaVipModel input)
        {
            //lay danh sách chăm sóc khách hàng
            string keyHu = CachingHandler.Instance.GeneralRedisKey("XocDiaVip", "Mode");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            int _Model = (input.Model == 1 ? 0 : (input.Model == 2 ? 1 : -1));
            _cachePvd.Set(keyHu, _Model);

            //lay danh sách chăm sóc khách hàng
            string keySession = "xocdiavip.session:info";

            RedisCacheProvider _cachePvd2 = new RedisCacheProvider();
            XocDiaVipRedisSession result = _cachePvd2.Get<XocDiaVipRedisSession>(keySession);
            if (result == null)
            {
                result = new XocDiaVipRedisSession();
            }

            return new JsonResult
            {
                Data = new
                {
                    IsOke = _cachePvd.Get<int>(keyHu) == _Model
                },
            };
        }
    }
}