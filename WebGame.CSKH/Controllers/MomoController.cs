using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Helpers;
using MsWebGame.CSKH.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Controllers
{
    public class MomoController : BaseController
    {
        #region lich sử nạp thẻ cào
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult Index(string nickName = null)
        {

            ViewBag.ServiceBox = GetServices();
            ViewBag.GetStatus = GetStatus();
            ViewBag.BankTypes = GetBankType();
            ViewBag.NickName = nickName;
            ViewBag.Partners = GetPartner(1);
            return View();
        }
        private List<SelectListItem> GetStatus()
        {
            return new List<SelectListItem>()
            {
            
                new SelectListItem() {Text="Thành công",Value="1" },
                new SelectListItem() {Text="Chờ xử lý",Value="0" },
                new SelectListItem() {Text="Gửi yêu cầu thất bại",Value="-1" },
                 new SelectListItem() {Text="Thất bại",Value="-2" },

            };
        }
        private List<SelectListItem> GetPartner(int SerViceID)
        {
            return new List<SelectListItem>()
            {

                new SelectListItem() {Text="HAPPY",Value="1" },
                new SelectListItem() {Text="SHOP THE NHANH",Value="2" },
             
            };
        }
        private List<SelectListItem> GetBankType()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() {Text="Nạp tiền",Value="1" },
                //new SelectListItem() {Text="Rút tiền",Value="2" }
            };
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult GetMomoList(long ? RequestID,string NickName, string RequestCode , string RefKey, string RefSendKey,
             DateTime? FromRequestDate , DateTime? ToRequestDate , int? Status, int? ServiceID, int? partnerID,string MomoReceive, int currentPage = 1)
        {



            if (string.IsNullOrEmpty(NickName))
                NickName = null;

            if (string.IsNullOrEmpty(RequestCode))
                RequestCode = null;
            if (string.IsNullOrEmpty(RefKey))
                RefKey = null;
            if (string.IsNullOrEmpty(RefSendKey))
                RefSendKey = null;
            if (string.IsNullOrEmpty(MomoReceive))
                MomoReceive = null;
            ViewBag.GetStatus = GetStatus();

            int totalRecord = 0;
            int customerPageSize = 25;

            currentPage = currentPage - 1 <= 0 ? 1 : currentPage;
            var lstCard = MOMODAO.Instance.UserMomoRequesList(RequestID,null, NickName, RequestCode, RefKey, RefSendKey, FromRequestDate, ToRequestDate, Status, ServiceID, partnerID,MomoReceive
               , currentPage, customerPageSize, out totalRecord);

            var pager = new Pager(totalRecord, (currentPage), customerPageSize, 10);
            //int totalPage = totalRecord / AppConstants.CONFIG.GRID_SIZE + 1;
            ViewBag.CurrentPage = pager.CurrentPage;
            ViewBag.TotalPage = pager.TotalPages;

            ViewBag.TotalRecord = pager.TotalItems;
            ViewBag.Prev = pager.Pre;
            ViewBag.Next = pager.Next;

            ViewBag.Start = pager.StartPage;
            ViewBag.End = pager.EndPage;
            ViewBag.StartIndex = pager.StartIndex + 1;
            ViewBag.EndIndex = pager.EndIndex + 1;
            ViewBag.TotalPage = pager.TotalPages;
            ViewBag.IsAdmin = Session["RoleCode"].ToString() == ADMIN_ROLE ? true : false;

            //int totalPage = totalRecord / AppConstants.CONFIG.GRID_SIZE + 1;
            //ViewBag.CurrentPage = currentPage;
            //ViewBag.TotalPage = totalPage;
            //ViewBag.TotalRecord = totalRecord;
            //ViewBag.Prev = currentPage != 1 ? currentPage - 1 : currentPage;
            //ViewBag.Next = currentPage == totalPage ? currentPage : currentPage + 1;
            //ViewBag.Start = (currentPage - 1) * 10 + 1;
            //ViewBag.End = currentPage == totalPage ? totalRecord : currentPage * 10;
            //ViewBag.IsAdmin = Session["RoleCode"].ToString() == "ADMIN" ? true : false;
            return PartialView(lstCard);
        }


        //[AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        //public ActionResult GetUSDTByID(long requestId)
        //{
        //    if (requestId <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Tham số không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    int TotalRecord = 0;
        //    var usdt = USDTDAO.Instance.UserBankRequestList(requestId, null, null, null, null, null, null, null, 1, 1, out TotalRecord).FirstOrDefault();
        //    if (usdt == null)
        //    {
        //        return Json(new
        //        {
        //            ResponseCode = -1,
        //            Message = "Tham số không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new
        //    {
        //        ResponseCode = 1,
        //        obj = new
        //        {
        //            usdt.DisplayName,
        //            RequestName = usdt.RequestType == 1 ? "Nạp tiền" : "Rút tiền",
        //            RequestCode = usdt.RequestCode,
        //            StatusStr = usdt.Status,
        //            AmountGame = usdt.RequestType == 1 ? usdt.ReceivedMoney : usdt.Amount,
        //            AmountUSDT = usdt.USDTAmount,
        //            AmountVND = usdt.RequestType == 1 ? usdt.Amount : usdt.ReceivedMoney,

        //        }
        //    }, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// hàm này check xem có refund ko 
        ///// </summary>
        ///// <param name="requestId"></param>
        ///// <param name="AmountGame"></param>
        ///// <param name="AmountUSDT"></param>
        ///// <param name="AmountVND"></param>
        ///// <param name="CheckNote"></param>
        ///// <returns></returns>
        //[AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        //public ActionResult CheckUSDT(long requestId, long AmountGame, double AmountUSDT, long AmountVND, string CheckNote, int Type)
        //{
        //    var acceptList = new List<String> { AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_CSKH_01, AppConstants.ADMINUSER.USER_CSKH_09 };
        //    if (!acceptList.Contains(AdminAccountName))
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Tài khoản không đủ quyền để thực hiện chứ năng này"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    var acceptTypeList = new List<int> { 1, 0 };
        //    if (!acceptTypeList.Contains(Type))
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Tham số type không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (requestId <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Tham số không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (AmountGame <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Giá trị Amount Game không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (AmountVND <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Giá trị VietNam đồng không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    if(AdminAccountName!= AppConstants.ADMINUSER.USER_ADMINTEST)
        //    {
        //        if (AmountVND >= 20000000)
        //        {
        //            return Json(new
        //            {
        //                Code = -1,
        //                Message = "Giá trị hoàn không được lớn hơn 20.000.000"
        //            }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
            
        //    if (AmountUSDT <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Giá trị USDT không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (String.IsNullOrEmpty(CheckNote))
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Không truyền giá trị note"
        //        }, JsonRequestBehavior.AllowGet);
        //    }


        //    int TotalRecord = 0;
        //    var usdt = USDTDAO.Instance.UserBankRequestList(requestId, null, null, null, null, null, null, null, 1, 1, out TotalRecord).FirstOrDefault();
        //    if (usdt == null)
        //    {
        //        return Json(new
        //        {
        //            ResponseCode = -1,
        //            Message = "Không tìm thấy bản ghi với mã Request ID " + requestId
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (usdt.RequestType == 1)
        //    {
        //        //check date
        //        if (!acceptList.Contains(AdminAccountName)){
        //            TimeSpan span = DateTime.Now.Subtract(usdt.RequestDate);
        //            if (span.TotalMinutes <= 130)
        //            {
        //                return Json(new
        //                {
        //                    ResponseCode = -1,
        //                    Message = "Thời gian hoàn hoặc hủy phải trên 130 phút"
        //                }, JsonRequestBehavior.AllowGet);

        //            }
        //        }
                
        //        var acceptStaus = new List<int> { 3, 5 };
        //        if ((!acceptStaus.Contains(usdt.Status)) && Type == 1)
        //        {
        //            return Json(new
        //            {
        //                ResponseCode = -1,
        //                Message = "Không thể hoàn tiền giao dịch khác trạng thái khác chờ feeback của ngân hàng hoặc thất bại"
        //            }, JsonRequestBehavior.AllowGet);
        //        }
        //        var acceptHuyStaus = new List<int> { 3 };
        //        if ((!acceptHuyStaus.Contains(usdt.Status)) && Type == 0)
        //        {
        //            return Json(new
        //            {
        //                ResponseCode = -1,
        //                Message = "Không thể hoàn tiền hủy khác trạng thái khác chờ feeback của ngân hàng "
        //            }, JsonRequestBehavior.AllowGet);
        //        }
        //        var GameRate = usdt.Rate;
        //        var usdtRate = usdt.ExchangeRate ?? 23500;
                

        //    }
        //    if (usdt.RequestType == 2)
        //    {
        //        return Json(new
        //        {
        //            ResponseCode = -1,
        //            Message = "Chưa phát triển chức nang hoàn cho giao dịch Rút"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    //hoàn tiền hay hủy
        //    var ajustStaus = Type == 1 ? 1 : 0;
        //    int Response = 0;
        //    long RemainBalance = 0;
        //    USDTDAO.Instance.UserBankRequestAdjust(usdt.RequestID, ajustStaus, usdt.ServiceID.Value, AmountUSDT, AdminID, usdt.UserID, AmountGame, AmountVND, CheckNote, CheckNote, out Response, out RemainBalance);
        //    if (Response == 1)
        //    {
        //        try
        //        {

                    
        //            SendDNA(usdt.ServiceID.Value, usdt.UserID,7, AmountVND, AmountVND);


        //        }
        //        catch (Exception ex)
        //        {
        //            NLogManager.PublishException(ex);
        //        }
        //        return Json(new
        //        {
        //            ResponseCode = 1,
        //            Message = "Thành công"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new
        //        {
        //            ResponseCode = 1,
        //            Message = "Hoàn tiền thất bại " + Response
        //        }, JsonRequestBehavior.AllowGet);
        //    }



        //}


        //[AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        //public ActionResult CalcuatelUSDT(long requestId, double AmountUSDT)
        //{

        //    if (requestId <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Tham số không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }


        //    if (AmountUSDT <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Giá trị USDT không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }


        //    int TotalRecord = 0;
        //    var usdt = USDTDAO.Instance.UserBankRequestList(requestId, null, null, null, null, null, null, null, 1, 1, out TotalRecord).FirstOrDefault();
        //    if (usdt == null)
        //    {
        //        return Json(new
        //        {
        //            ResponseCode = -1,
        //            Message = "Tham số không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    var GameRate = usdt.Rate;
        //    var usdtRate = usdt.ExchangeRate ?? 23500;
        //    double AmountVND = Math.Ceiling(AmountUSDT * usdtRate);
        //    double AmountGame = Math.Ceiling((double)(AmountVND * GameRate));

        //    return Json(new
        //    {
        //        ResponseCode = 1,
        //        AmountVND = AmountVND,
        //        AmountGame = AmountGame,
        //    }, JsonRequestBehavior.AllowGet);
        //}




        #endregion lịch sử nạp thẻ cào
    }
}