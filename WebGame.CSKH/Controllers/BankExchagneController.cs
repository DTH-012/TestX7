﻿using MsTraditionGame.Utilities.Messages;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Helpers;
using MsWebGame.CSKH.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Exchanges;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.MyUSDT.Charges;

namespace MsWebGame.CSKH.Controllers
{
    public class BankExchagneController : BaseController
    {
        protected static string PENDING = "pending";//Đang chờ xử lý, khi mới tạo order
        protected static string PROCESSING = "processing";//Sau khi tiền/USDT nộp vào tk thành công
        protected static string COMPLETED = "completed";//xử lý thành công
        protected static string CANCELED = "canceled";//order bị cancel sau khi hết thừoi gian xử lý
        protected static string FAILED = "failed";//Order bị lỗi trogn quá tình xử lý
        private int PartnerID = 1;
        protected int WaitBank_Success = 3;
        protected int Bank_Fail = 0;
        protected int PENDING_STAUTS = 0;
        protected int CONFIRM_STAUTS = 2;
        protected int APPROVED_STAUTS = 3;
        protected int FAIL_API1 = -1;
        protected int FAIL_API2 = -2;
        protected int SUCCESS_STAUTS = 1;
        protected int CANCELLED_STAUTS = 4;
        protected int REFUSED_STAUTS = 5;
        protected static int CHECKER_ID = 5;
        #region lich sử nạp thẻ cào
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult Index(string nickName = null)
        {

            ViewBag.ServiceBox = GetServices();
            ViewBag.GetStatus = GetStatus();
            ViewBag.BankTypes = GetBankType();
            ViewBag.NickName = nickName;
            return View();
        }
        private List<SelectListItem> GetStatus()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() {Text="Thất bại-Order (Đã hoàn tiền cho khách Q)",Value="-1" },
                 new SelectListItem() {Text="Thất bại-Chuyển USDT (Admin confirm xử lý)",Value="-2" },
                new SelectListItem() {Text="Chờ xử lý",Value="0" },
                new SelectListItem() {Text="Thành công",Value="1" },
                new SelectListItem() {Text="Chờ phản hồi từ ngân hàng",Value="3" },

                new SelectListItem() {Text="Admin hủy rút (Đã hoàn tiền Q) ",Value="4" },
                new SelectListItem() {Text="Ngân hàng từ chối(Đã hoàn tiền Q)",Value="5" },
           
            };
        }
        private List<SelectListItem> GetBankType()
        {
            return new List<SelectListItem>()
            {
              
                new SelectListItem() {Text="Rút tiền",Value="2" }
            };
        }

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost]
        public ActionResult UserUSDTExamine(long RequestID, long userId, int checkStatus)
        {
            long balance = 0;
            var usdt = USDTDAO.Instance.UserBankRequestGetByID(RequestID);
            if (usdt == null)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Không tồn tại bản ghi để xử lý"
                }, JsonRequestBehavior.AllowGet);

            }
            if (usdt.Status != 0)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Trạng thái phê duyệt của USDT không hợp lệ !=0"
                }, JsonRequestBehavior.AllowGet);

            }
            if (usdt.UserID != userId)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Tài khoản hoàn tiền không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
            //số tiền VND cần rút
            double realUSDT= usdt.ReceivedMoney.Value*1.0 / 23500;
            if (checkStatus == 1)
            {
                return CreateSellOrders(RequestID);
            }
            else
            {
                int res = 0;
                int REJECT = 4;
                USDTDAO.Instance.UserBankRequestExamine(RequestID, userId, null, REJECT, 0, 0, 0, AdminID,0,null, null
                    , null,null,null,null,null
                    , out res, out balance);


                if (res == 1)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UpdateSuccess
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -35)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardSwapNotFound
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -36)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserInValid
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -504)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotValid
                    }, JsonRequestBehavior.AllowGet);

                }
                return Json(new
                {
                    ResponseCode = res,
                    Message = ErrorMsg.InProccessException
                }, JsonRequestBehavior.AllowGet);
            }






        }

        private ActionResult CreateSellOrders(long RequestID)
        {
            try
            {



                //Lấy thông tin BankName (đã chọn )



                var feeObjet = BankChargeApiApi.GetRateConfig();
                if (feeObjet == null)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Hệ thống kết nối ngân hàng lỗi",
                    });

                }
                if (feeObjet.WithdrawFeeFlat<=0)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Lỗi kết nố api.feeObjet.WithdrawFeeFlat<=0",
                    });

                }
                if (feeObjet.WithdrawFeePct <= 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Lỗi kết nố api.feeObjet.WithdrawFeePct<=0",
                    });

                }
                if (feeObjet.SellRate <= 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Lỗi kết nố api.feeObjet.SellRate<=0",
                    });

                }

                long lngRequestID = ConvertUtil.ToLong(RequestID);
              

                //1.Lấy ra order details từ dbs (accountId,orderCode)
                var orderByCode = USDTDAO.Instance.UserBankRequestGetByID(lngRequestID);
                if (orderByCode == null)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = String.Format("Không tồn tại giao dịch theo mã {0}", lngRequestID),
                    });
                }

                //Tiền VND
                if (orderByCode.Amount <= 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = "Giá trị yêu cập  nạp không tồn tại",
                    });
                }
               
                var realMoney = orderByCode.ReceivedMoney.Value;//lấy ra số tiền VND (user muốn rút )
                var realFee = realMoney * feeObjet.WithdrawFeePct + feeObjet.WithdrawFeeFlat * feeObjet.SellRate;
                long target_amount =Convert.ToInt64(realMoney + realFee);


                var response = BankExchangesApi.SendSellOrder(target_amount, orderByCode.BankNumber, orderByCode.BankAccount, orderByCode.BankName);
                LoggerHelper.LogUSDTMessage(String.Format("BankExchangesApi.SendSellOrder(RequestID:{0})==>TargetAmount:{1}| BankAccount:{2}| BankAccountName:{3}| BankName:{4}|Result:{5}", RequestID, target_amount, orderByCode.BankNumber, orderByCode.BankAccount, orderByCode.BankName, JsonConvert.SerializeObject(response, Formatting.None)));

               
                if (response!=null&&response.Status == PENDING && !String.IsNullOrEmpty(response.Code))
                {
                    int Response = 0;
                 
                    //Gọi tiếp api số 2

                    double real_USDT = Math.Round(target_amount*1.0/ feeObjet.SellRate, 8);//lấy ra số tiền VND;
                    var address = response.Note.ReceiveUsdtAddress;

                    //var transactionRessponse = new MerchantTransactionsResponseModel();
                    //transactionRessponse.Id = 200;
                    var transactionRessponse = BankExchangesApi.MerchantTransactions(real_USDT, address);
                    LoggerHelper.LogUSDTMessage(String.Format("BankExchangesApi.MerchantTransactions(RequestID:{0})==>Amount:{1}| Result:{2}", RequestID, real_USDT, JsonConvert.SerializeObject(transactionRessponse, Formatting.None)));
                    if (transactionRessponse != null && transactionRessponse.Id > 0 )
                    {

                        string description = string.Empty;

                        long RemainBlance = 0;
                      
                        int AdminID = 5;
                        //USDTDAO.Instance.UserBankRequestExamine(orderByCode.RequestID, orderByCode.UserID, response.Code, WaitBank_Success, orderByCode.ServiceID.Value, response.AmountVcc, 
                        //    transactionRessponse.RealAmount, AdminID,
                        //    orderByCode.ReceivedMoney.Value, transactionRessponse.Amount,
                        //    response.Note.ReceiveUsdtAddress
                        //    , response.Status, response.HttpStatusCode.ToString(), response.HttpStatusMsg, null, null
                        //    , out Response, out RemainBlance);


                        USDTDAO.Instance.UserBankRequestUpdate2(orderByCode.RequestID, response.Code, orderByCode.UserID, PartnerID
                            , orderByCode.ServiceID.Value, WaitBank_Success, response.Status, null, null,null, transactionRessponse.Amount, null, null
                            ,null, null, null, response.HttpStatusCode.ToString(), null,response.HttpStatusMsg, null,null
                            , response.AmountVcc,null
                            , orderByCode.ReceivedMoney.Value, response.Note.ReceiveUsdtAddress, out Response);


                        
                        LoggerHelper.LogUSDTMessage(String.Format("USDTDAO.Instance.UserBankRequestExamine.Success==> RequestID:{0}|UserID:{1}|RequestCode:{2}|CheckStatus:{3}|ServiceID:{4}|RealAmount:{5}|RealUSDTAmount:{6}|ExamineUserID:{7}|RealReceivedMoney:{8}|USDTWalletAddress:{9}|Response:{10}|RemainBalance:{11}", orderByCode.RequestID, orderByCode.UserID, response.Code, WaitBank_Success, orderByCode.ServiceID.Value, response.AmountVcc, response.AmountVcc, 1, 0, response.Note.ReceiveUsdtAddress, Response, RemainBlance));
                        if (Response == 1)
                        {
                            return Json(new
                            {
                                ResponseCode = 1,
                                Message = "Duyệt thành công-đã gửi yêu cầu chuyển khoản ngân hàng."
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                ResponseCode = -1005,
                                Message = "Lỗi khi cập nhật thông tin Order" + Response
                            });
                        }
                    }
                    else
                    {
                        //lỗi api số 2 
                        USDTDAO.Instance.UserBankRequestUpdateStatus2(orderByCode.RequestID, response.Code, orderByCode.ServiceID.Value, ((int)transactionRessponse.HttpStatusCode).ToString(), null, (transactionRessponse.HttpMsg).ToString(), null, FAIL_API2,0, orderByCode.UserID, response.Status, null, null, null,null,null, response.Note.ReceiveUsdtAddress, PartnerID, out Response);
                        LoggerHelper.LogUSDTMessage(String.Format("UserBankRequestUpdateStatus.API_2 RequestID:{0}| RequestCode:{1}|ServiceID:{2}| PartnerErrorCode:{3}| FeedbackErrorCode:{4}|PartnerMessage:{5}|FeedbackMessage:{6}|Status:{7}|TransReqStatus:{8}|Response:{9}", orderByCode.RequestID, null, orderByCode.ServiceID.Value, ((int)response.HttpStatusCode).ToString(), null, response.HttpStatusMsg, null, FAIL_API1, CONFIRM_STAUTS, Response));
                        //Lỗi chưa xử lý

                        return Json(new
                        {
                            ResponseCode = -10005,
                            Message = "Lỗi kết nối api số 2 -liên hệ USDT để xử lý "
                        });

                    }


                }
                else
                {
                  
                    //lỗi api số 1 ko lấy được thông tin 
                    //USDTDAO.Instance.UserBankRequestUpdateStatus(orderByCode.RequestID, null, orderByCode.ServiceID.Value, ((int)response.HttpStatusCode).ToString(), null, response.HttpStatusMsg, null, FAIL_API1, CONFIRM_STAUTS, orderByCode.UserID, null,fee,null,null,null,null,null,PartnerID, out Response);
                    //LoggerHelper.LogUSDTMessage(String.Format("UserBankRequestUpdateStatus.API_1  RequestID:{0}| RequestCode:{1}|ServiceID:{2}| PartnerErrorCode:{3}| FeedbackErrorCode:{4}|PartnerMessage:{5}|FeedbackMessage:{6}|Status:{7}|TransReqStatus:{8}|Response:{9}", orderByCode.RequestID, null, orderByCode.ServiceID.Value, ((int)response.HttpStatusCode).ToString(), null, response.HttpStatusMsg, null, FAIL_API1, CONFIRM_STAUTS, Response));

               
                    int res = 0;
                    long balance = 0;
                    int API_1_FAIL = -1;


                    USDTDAO.Instance.UserBankRequestExamine(RequestID, orderByCode.UserID,response.Code,API_1_FAIL, orderByCode.ServiceID.Value,0, 0, AdminID, 0, null,null
                        , response.Status, response.HttpStatusCode.ToString(), response.HttpStatusMsg, null, null,
                        out res, out balance);
                    //cập nhật lại trạng thái của order 
                    //LoggerHelper.LogUSDTMessage(String.Format("BankCharge.Exchange.Failed-RequestID:{0}| UserID:{1}| CheckStatus:{2}|ServiceID:{3}| RealUSDTAmount{4}|RequestRate:{5}|CheckerID:{6}|RequestAmount:{7}| RealAmount:{8}|RealReceivedMoney:{9}| string PartnerStatus{10}|RemainBalance:{11}| Response:{12}",
                    //orderByCode.RequestID, orderByCode.UserID, Bank_Fail, ServiceID, model.AmountUsdt, model.Rate, CHECKER_ID, 0, model.AmountVcc, realAmountReceive, model.Status, RemainBalance, Response
                    //   ));
                    return Json(new
                    {
                        ResponseCode = -10005,
                        Message = "Hệ thống ngân hàng kết nối lỗi-Chúng tôi đã hoàn tiền cho bạn"
                    });

                }






            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return Json(new
                {
                    ResponseCode = -99,
                    Description = "Hệ thống đang bận, vui lòng quay lại sau"
                });
            }
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult GetUSTDList(string nickName = null, string code = null, int? type = 2,
             DateTime? fromDate = null, DateTime? toDate = null, int? status = null, int? serviceId = null, int currentPage = 1)
        {


            if (string.IsNullOrEmpty(nickName))
                nickName = null;

            if (string.IsNullOrEmpty(code))
                code = null;
            ViewBag.GetStatus = GetStatus();

            int totalRecord = 0;
            int customerPageSize = 25;
            type = 2;

            currentPage = currentPage - 1 <= 0 ? 1 : currentPage;
            var lstCard = USDTDAO.Instance.UserBankRequestList(null, nickName, code, fromDate, toDate, status, serviceId, type
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


        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult GetUSDTByID(long requestId)
        {
            if (requestId <= 0)
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Tham số không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }

            int TotalRecord = 0;
            var usdt = USDTDAO.Instance.UserBankRequestList(requestId, null, null, null, null, null, null, null, 1, 1, out TotalRecord).FirstOrDefault();
            if (usdt == null)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = "Tham số không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                ResponseCode = 1,
                obj = new
                {
                    usdt.DisplayName,
                    RequestName = usdt.RequestType == 1 ? "Nạp tiền" : "Rút tiền",
                    RequestCode = usdt.RequestCode,
                    StatusStr = usdt.Status,
                    AmountGame = usdt.ReceivedMoney,
                    AmountUSDT = usdt.USDTAmount,
                    AmountVND = usdt.Amount,

                }
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// hàm này check xem có refund ko 
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="AmountGame"></param>
        /// <param name="AmountUSDT"></param>
        /// <param name="AmountVND"></param>
        /// <param name="CheckNote"></param>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult CheckUSDT(long requestId, string CheckNote, int Type)
        {
            var acceptList = new List<String> { AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_CSKH_01, AppConstants.ADMINUSER.USER_CSKH_09 };
            if (!acceptList.Contains(AdminAccountName))
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Tài khoản không đủ quyền để thực hiện chứ năng này"
                }, JsonRequestBehavior.AllowGet);
            }
            var acceptTypeList = new List<int> { 1, 0 };
            if (!acceptTypeList.Contains(Type))
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Tham số type không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
            if (requestId <= 0)
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Tham số không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
          
           
            if (String.IsNullOrEmpty(CheckNote))
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Không truyền giá trị note"
                }, JsonRequestBehavior.AllowGet);
            }


          
            var usdt = USDTDAO.Instance.UserBankRequestGetByID(requestId);
            if (usdt == null)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = "Không tìm thấy bản ghi với mã Request ID " + requestId
                }, JsonRequestBehavior.AllowGet);
            }
           
            if (usdt.RequestType != 2)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = "Không được hoàn tiền ngoài giao dịch rút " + requestId
                }, JsonRequestBehavior.AllowGet);
            }
            if (usdt.RequestType == 2)
            {
                //check date
                if (!acceptList.Contains(AdminAccountName))
                {
                    TimeSpan span = DateTime.Now.Subtract(usdt.RequestDate);
                    if (span.TotalMinutes <= 20)
                    {
                        return Json(new
                        {
                            ResponseCode = -1,
                            Message = "Thời gian hoàn hoặc hủy phải trên 20 phút"
                        }, JsonRequestBehavior.AllowGet);

                    }
                }

                var acceptStaus = new List<int> { -2};
                if ((!acceptStaus.Contains(usdt.Status)) && Type == 1)
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Không được hoàn tiền với request khác với chờ admin xử lý"
                    }, JsonRequestBehavior.AllowGet);
                }
                var acceptHuyStaus = new List<int> { -2};
                if ((!acceptHuyStaus.Contains(usdt.Status)) && Type == 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Không được hoàn tiền với request khác với chờ admin xử lý"
                    }, JsonRequestBehavior.AllowGet);
                }
               


            }
            //cập nhật trạng thái lại về 1 và không hoàn tiền cho khách,cập nhật lại trạng thái USDT,số USDT
            if (Type == 1)
            {
                var feeObjet = BankChargeApiApi.GetRateConfig();
                if (feeObjet == null)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Hệ thống kết nối ngân hàng lỗi",
                    });

                }
                if (feeObjet.WithdrawFeeFlat <= 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Lỗi kết nố api.feeObjet.WithdrawFeeFlat<=0",
                    });

                }
                if (feeObjet.WithdrawFeePct <= 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Lỗi kết nố api.feeObjet.WithdrawFeePct<=0",
                    });

                }
                if (feeObjet.SellRate <= 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Lỗi kết nố api.feeObjet.SellRate<=0",
                    });

                }
                var realMoney = usdt.ReceivedMoney.Value;//lấy ra số tiền VND (user muốn rút )
                var realFee = realMoney * feeObjet.WithdrawFeePct + feeObjet.WithdrawFeeFlat * feeObjet.SellRate;
                long target_amount = Convert.ToInt64(realMoney + realFee);
                double real_USDT = Math.Round(target_amount * 1.0 / feeObjet.SellRate, 8);//lấy ra số tiền VND;

                var ordersApi = BankChargeApiApi.GetOrderDeails(usdt.RequestCode);
                if (ordersApi == null)
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Không thể lấy được thông tin Order từ đối tác"
                    }, JsonRequestBehavior.AllowGet);
                }
                if (ordersApi.Status != COMPLETED)
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Đối tác đang trả về mã khác"+ COMPLETED
                    }, JsonRequestBehavior.AllowGet);
                }
                int Response = 0;
                long RemainBalance = 0;
                string format = String.Format("Admin xủ lý cập nhật thành công {0}",CheckNote);
                USDTDAO.Instance.UserBankRequestExchangeAdjust(usdt.RequestID, 1, usdt.ServiceID.Value,real_USDT, AdminID, usdt.UserID, CheckNote, format, out Response, out RemainBalance);



                if (Response == 1)
                {
                    
                    return Json(new
                    {
                        ResponseCode = 1,
                        Message = "Thành công"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        ResponseCode = 1,
                        Message = "Hoàn tiền thất bại " + Response
                    }, JsonRequestBehavior.AllowGet);
                }

            }else
            {

                int Response = 0;
                long RemainBalance = 0;
                string format = String.Format("Admin xủ lý cập nhật thành công {0}", CheckNote);
                USDTDAO.Instance.UserBankRequestExchangeAdjust(usdt.RequestID, 0, usdt.ServiceID.Value,null, AdminID, usdt.UserID, CheckNote, format, out Response, out RemainBalance);
                if (Response == 1)
                {
                    
                    return Json(new
                    {
                        ResponseCode = 1,
                        Message = "Thành công"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        ResponseCode = 1,
                        Message = "Hoàn tiền thất bại " + Response
                    }, JsonRequestBehavior.AllowGet);
                }



            }

           


        }


        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult CalcuatelUSDT(long requestId)
        {

            if (requestId <= 0)
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Tham số không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }


           


            
            var usdt = USDTDAO.Instance.UserBankRequestGetByID(requestId);
            if (usdt == null)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = "Tham số không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
            var feeObjet = BankChargeApiApi.GetRateConfig();
            if (feeObjet == null)
            {
                return Json(new
                {
                    ResponseCode = -1006,
                    Message = "Hệ thống kết nối ngân hàng lỗi",
                });

            }
            if (feeObjet.WithdrawFeeFlat <= 0)
            {
                return Json(new
                {
                    ResponseCode = -1006,
                    Message = "Lỗi kết nố api.feeObjet.WithdrawFeeFlat<=0",
                });

            }
            if (feeObjet.WithdrawFeePct <= 0)
            {
                return Json(new
                {
                    ResponseCode = -1006,
                    Message = "Lỗi kết nố api.feeObjet.WithdrawFeePct<=0",
                });

            }
            if (feeObjet.SellRate <= 0)
            {
                return Json(new
                {
                    ResponseCode = -1006,
                    Message = "Lỗi kết nố api.feeObjet.SellRate<=0",
                });

            }

        


          
         
            var realMoney = usdt.ReceivedMoney.Value;//lấy ra số tiền VND (user muốn rút )
            var realFee = realMoney * feeObjet.WithdrawFeePct + feeObjet.WithdrawFeeFlat * feeObjet.SellRate;
            long target_amount = Convert.ToInt64(realMoney + realFee);
            double real_USDT = Math.Round(target_amount * 1.0 / feeObjet.SellRate, 8);//lấy ra số tiền VND;

            //var ordersApi = BankChargeApiApi.GetOrderDeails(usdt.RequestCode);
            //if (ordersApi == null)
            //{
            //    return Json(new
            //    {
            //        ResponseCode = -1,
            //        Message = "Không thể lấy được thông tin Order từ đối tác"
            //    }, JsonRequestBehavior.AllowGet);
            //}

            return Json(new
            {
                ResponseCode = 1,
                Amount = real_USDT,
             
            }, JsonRequestBehavior.AllowGet);
        }




        #endregion lịch sử nạp thẻ cào
    }
}