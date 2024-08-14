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
    public class CcuController : BaseController
    {
        private List<string> accpetList = new List<string> { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINREF, AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_CSKH_01 };
        // GET: CustomerSupport
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Index()
        {
            DateTime date = DateTime.Now;
            DateTime dateStart = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
            DateTime dateEnd = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
            List<CuuListModel> Lists = CcuDAO.Instance.GetLists(dateStart, dateEnd);
            List<object[]> list = new List<object[]>();
            foreach (CuuListModel val in Lists)
            {
                list.Add(new object[] { val.Time, val.Web, val.Android,val.Ios, val.Total });
            }
             
            ViewBag.DateTimeNow = DateTime.Now.ToString("yyyy-MM-dd");
            return View(list);
        }
    }
}