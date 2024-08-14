using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MsTraditionGame.Utilities.Utils;
using Telerik.Web.Mvc;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Security;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Helpers;
using MsWebGame.CSKH.Models.Accounts;
using MsWebGame.CSKH.Models.HistoryTranfers;
using MsWebGame.CSKH.Models.Param;
using MsWebGame.CSKH.Models.Transactions;
using MsWebGame.CSKH.Utils;
using TraditionGame.Utilities.OneSignal;

namespace MsWebGame.CSKH.Controllers
{
    public class AnalyticsController : BaseController
    {
        private List<string> accpetList = new List<string> { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINREF, AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_CSKH_01 };
        // GET: CustomerSupport
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Index()
        {
            DateTime date = DateTime.Now;
            date = date.AddDays(-6);
            DateTime dateStart = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
            DateTime dEnd = DateTime.Now;
            DateTime dateEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59, 999);
            
            List<CasoutPay> UserCardRequest = AnalyticsDAO.Instance.GetCasoutPay(dateStart, dateEnd, "SP_UserCardRequest_Sum");
            List<CasoutPay> UserMomoRequest = AnalyticsDAO.Instance.GetCasoutPay(dateStart, dateEnd, "SP_UserMomoRequest_Sum");
            List<CasoutPay> UserBankRequest = AnalyticsDAO.Instance.GetCasoutPay(dateStart, dateEnd, "SP_UserBankRequest_Sum");
            
            List<Dictionary<string,string>> list = new List<Dictionary<string, string>>();
            ViewBag.StartDate = dateStart.ToString("dd/MM/yyyy HH:ss");
            ViewBag.EndDate = dateEnd.ToString("dd/MM/yyyy HH:ss");
            for (int i = 0 ; i <  7; i++)
            {
                DateTime _date = date.AddDays(i);
                string dateEqua = _date.ToString("dd/MM/yyyy");
                //{"totalCharginSMS":0,"date2":"2021-08-20 14:30:59 - 2021-08-21 14:30:59","totalCharginCard":"20390000","totalSpin":0,"totalCharginIAP":"184000","totalCharginTopup":"6450000","totalUser":"711","date":"2021-08-20"}
                long totalCharginCard = 0;
                long totalCharginMomo = 0;
                long totalCharginBank = 0;
                foreach (CasoutPay val in UserCardRequest)
                {
                    
                    if (val.CreateUpdate == dateEqua)
                    {
                        totalCharginCard += val.Money;
                    }
                }
                foreach (var val in UserMomoRequest)
                {
                     
                    if (val.CreateUpdate == dateEqua)
                    {
                        totalCharginMomo += val.Money;
                    }
                }
                foreach (var val in UserBankRequest)
                {
                     
                    if (val.CreateUpdate == dateEqua)
                    {
                        totalCharginBank += val.Money;
                    }
                }
                list.Add(new Dictionary<string, string>() {
                    { "date",_date.ToString("yyyy-MM-dd") },
                    { "totalCharginCard",totalCharginCard.ToString() },
                    { "totalCharginMomo",totalCharginMomo.ToString() },
                    { "totalCharginBank",totalCharginBank.ToString() },
                    { "totalOutBank", "0" },
                });
            }
            return View(list);
        }
    }
}