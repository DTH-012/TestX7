﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Models.XocDia;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class XocDiaDAO
    {
        private static readonly Lazy<XocDiaDAO> _instance = new Lazy<XocDiaDAO>(() => new XocDiaDAO());

        public static XocDiaDAO Instance
        {
            get { return _instance.Value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<EventModel> GetEventList()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.SicboConn);
                return db.GetListSP<EventModel>("SP_EventConfig_GetList");
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }
        public List<SoiCau> GetSoiCau(int top)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.SicboConn);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_TopCount", top);


                var lstRs = db.GetListSP<SoiCau>("SP_GetSoiCau", pars);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        public EventModel GetEventInfo(long eventID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.SicboConn);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_EventID", eventID);
                var list = db.GetListSP<EventModel>("SP_EventConfig_GetInfo", pars);
                if (list != null && list.Any()) return list.FirstOrDefault();
                return new EventModel();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new EventModel();
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public int SaveEvent(EventModel model)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.SicboConn);
                var pars = new SqlParameter[13];
                pars[0] = new SqlParameter("@_EventID", model.EventID);
                pars[1] = new SqlParameter("@_BetValueMin", model.BetValueMin);
                pars[2] = new SqlParameter("@_QuantityCordWin", model.QuantityCordWin);
                pars[3] = new SqlParameter("@_QuantityCordLost", model.QuantityCordLost);
                pars[4] = new SqlParameter("@_PrizeValueMin", model.PrizeValueMin);
                pars[5] = new SqlParameter("@_PrizeValueMax", model.PrizeValueMax);
                pars[6] = new SqlParameter("@_QuantityAwardCordWinInit", model.QuantityAwardCordWinInit);
                pars[7] = new SqlParameter("@_QuantityAwardCordLostInit", model.QuantityAwardCordLostInit);
                pars[8] = new SqlParameter("@_StartEventTimes", model.StartEventTimes);
                pars[9] = new SqlParameter("@_EndEventTimes", model.EndEventTimes);
                pars[10] = new SqlParameter("@_EventDays", model.EventDays);
                pars[11] = new SqlParameter("@_Hours", model.Hours);
                pars[12] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_EventConfig_Save", pars);
                return Int32.Parse(pars[12].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public int DeleteEvent(EventModel model)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.SicboConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_EventID", model.EventID);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_EventConfig_Delete", pars);
                return Int32.Parse(pars[1].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public List<RaceTopModel> GetRaceTopList()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.SicboConn);
                return db.GetListSP<RaceTopModel>("SP_RaceTopConfig_GetList");
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public RaceTopModel GetRaceTopInfo(long raceTopID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.SicboConn);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_RaceTopID", raceTopID);
                var list = db.GetListSP<RaceTopModel>("SP_RaceTopConfig_GetInfo", pars);
                if (list != null && list.Any()) return list.FirstOrDefault();
                return new RaceTopModel();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new RaceTopModel();
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public List<XocDiaRefunds> GetRefundsInfo(long uid, long session)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.SicboConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountID", uid);
                pars[1] = new SqlParameter("@_SessionID", session);
                var list = db.GetListSP<XocDiaRefunds>("SP_GetRefundsInfo", pars);
                return list;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public int SaveRaceTop(RaceTopModel model)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.SicboConn);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_RaceTopID", model.RaceTopID);
                pars[1] = new SqlParameter("@_Quantity", model.Quantity);
                pars[2] = new SqlParameter("@_PrizeValue", model.PrizeValue);
                pars[3] = new SqlParameter("@_Description", model.Description);
                pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_RaceTopConfig_Save", pars);
                return Int32.Parse(pars[4].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public int DeleteRaceTop(RaceTopModel model)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.SicboConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_RaceTopID", model.RaceTopID);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_RaceTopConfig_Delete", pars);
                return Int32.Parse(pars[1].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public List<ReportEventModel> GetEventAwardList(ReportEventModel model, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceLogConn);
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_EventID", model.EventID);
                pars[1] = new SqlParameter("@_RaceTopID", model.RaceTopID);
                pars[2] = new SqlParameter("@_Username", model.Username);
                pars[3] = new SqlParameter("@_CordWinOrLost", model.CordWinOrLost);
                pars[4] = new SqlParameter("@_StartDate", model.StartDate);
                pars[5] = new SqlParameter("@_EndDate", model.EndDate);
                pars[6] = new SqlParameter("@_CurrentPage", currentPage);
                pars[7] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[8] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var list = db.GetListSP<ReportEventModel>("SP_EventAward_GetList", pars);
                totalRecord = Convert.ToInt32(pars[8].Value);
                return list;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalRecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }
        public List<UserBetModel> GetUsersSession(long SessionId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.SicboConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_SessionId", SessionId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                return db.GetListSP<UserBetModel>("SP_GetUsersSession", pars);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new List<UserBetModel>();
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }
    }
}