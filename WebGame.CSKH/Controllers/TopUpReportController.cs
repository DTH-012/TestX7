﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;

namespace MsWebGame.CSKH.Controllers
{
    public class TopUpReportController : BaseController
    {
        // GET: TopUpReport
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult Index(DateTime? FromRequestDate, DateTime? ToRequestDate)
        {
            ViewBag.FromDate = DateTime.Now;
            ViewBag.ToDate = DateTime.Now;
            if (FromRequestDate == null)
            {
                FromRequestDate = DateTime.Now;
                ToRequestDate = DateTime.Now;
            }
            var data = CardDAO.Instance.GetTopUpReport(FromRequestDate, ToRequestDate, 1);
            if (data != null)
            {
                ViewBag.Viettel = data.FirstOrDefault(c => c.TelOperatorID == 1)!= null?data.FirstOrDefault(c => c.TelOperatorID == 1).CardValue:0;
                ViewBag.VinaPhone = data.FirstOrDefault(c => c.TelOperatorID == 2)!= null?data.FirstOrDefault(c => c.TelOperatorID == 2).CardValue:0;
                ViewBag.MobiPhone = data.FirstOrDefault(c => c.TelOperatorID == 3)!= null?data.FirstOrDefault(c => c.TelOperatorID == 3).CardValue:0;
                ViewBag.Momo = data.FirstOrDefault(c => c.TelOperatorID == 4)!= null?data.FirstOrDefault(c => c.TelOperatorID == 4).CardValue:0;
                ViewBag.ViettelPay = data.FirstOrDefault(c => c.TelOperatorID == 5)!= null?data.FirstOrDefault(c => c.TelOperatorID == 5).CardValue:0;
                ViewBag.Bank = data.FirstOrDefault(c => c.TelOperatorID == 6) != null ? data.FirstOrDefault(c => c.TelOperatorID == 6).CardValue : 0;
                ViewBag.Momoout = data.FirstOrDefault(c => c.TelOperatorID == 7) != null ? data.FirstOrDefault(c => c.TelOperatorID == 7).CardValue : 0;
                ViewBag.Bankout = data.FirstOrDefault(c => c.TelOperatorID == 8) != null ? data.FirstOrDefault(c => c.TelOperatorID == 8).CardValue : 0;
                ViewBag.Total = ViewBag.Viettel + ViewBag.VinaPhone + ViewBag.MobiPhone + ViewBag.Momo +
                                ViewBag.ViettelPay + ViewBag.Bank;
                ViewBag.Totalout = ViewBag.Momoout + ViewBag.Bankout;
                ViewBag.TotalInOutMomo = ViewBag.Momo - ViewBag.Momoout;
                ViewBag.TotalInOutBank = ViewBag.Bank - ViewBag.Bankout;
                ViewBag.TotalChenhLech = ViewBag.Total - ViewBag.Totalout;
            }
            
            return View();
        }


    }
}