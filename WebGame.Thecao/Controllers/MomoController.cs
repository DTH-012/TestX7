using MsWebGame.Thecao.Database.DAO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MsWebGame.Thecao.Database.DTO;
using TraditionGame.Utilities;
using TraditionGame.Utilities.DNA;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.Momos.Api.Charges;
using TraditionGame.Utilities.Momos.Models;
using System.Data.SqlClient;
using System.Data;
using TraditionGame.Utilities.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/Momo")]
    public class MomoController : BaseApiController
    {
        private string MOMO_SECRETKEY = ConfigurationManager.AppSettings["MOMO_SECRETKEY"].ToString();
        private List<string> AccpetProvider = new List<string>() { "MMO" };



        public static void UpdateStatusMomo(string @_requestId, out int Response)
        {
            DBHelper db = null;
            Response = -99;
            try
            {
                db = new DBHelper(Config.BettingConn);
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_UserId", SqlDbType.BigInt);
                param[0].Value = 0;

                param[1] = new SqlParameter("@_Id", SqlDbType.BigInt);
                param[1].Value = 0;

                param[2] = new SqlParameter("@_Status", SqlDbType.BigInt);
                param[2].Value = 1;

                param[3] = new SqlParameter("@_requestId", SqlDbType.BigInt);
                param[3].Value = @_requestId;

                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;


                db.ExecuteNonQuerySP("SP_Casout_Momo_UpdateStatus", param.ToArray());

                Response = Convert.ToInt32(param[3].Value);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        [ActionName("CallBackResultVTP")]
        [HttpPost]
        public HttpResponseMessage CallBackResultVTP(string transaction_id,string phone,string money,string signature)
        {
            NLogManager.LogMessage(String.Format("Nạp ViettelPay-transaction_id:{0}-phone:{1}-money:{2}-signature:{3}",transaction_id,phone,money,signature));
            return JsonMomoResult(200, "Nạp thành công");
        }

        [ActionName("MomoCallBackResultAction")]
        [HttpGet]

        public HttpResponseMessage MomoCallBackResultAction(int chargeId, string chargeType, string chargeCode, int regAmount, string status, int chargeAmount,string requestId, string signature, string momoTransId)
        {
            //https://napthe.sanhux6.club/api/Momo/MomoCallBackResultAction?chargeId=5757099&chargeType=momo&chargeCode=08cu4568&regAmount=100000&status=success&chargeAmount=100000&requestId=vvvvvv&signature=b57ce88fac8904d16929caa36a507962&momoTransId=null46939401994
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            string IpsMomo = System.Configuration.ConfigurationManager.AppSettings["IpsMomo"];

            if (!string.IsNullOrEmpty(IpsMomo) && ip != IpsMomo)
            {
                var msg = String.Format("{0}|{1}", "-1", "Ip Invalid");
                return JsonMomoResult(-106, msg);
            }

            //if (momoRequest == null)
            //{
            //    return JsonMomoResult(-100, "ParaInvalid model null");
            //}
            //LoggerHelper.LogUSDTMessage(String.Format("MomoCharge.CallBackResult CallBackModel:{0}", JsonConvert.SerializeObject(momoRequest)));

            //if (string.IsNullOrEmpty(momoTransId))
            //{
            //    return JsonMomoResult(-105, "MomoId  null");
            //}
            if (String.IsNullOrEmpty(chargeCode))
            {
                return JsonMomoResult(-106, "Message null");
            }
            if (regAmount <= 0 || regAmount < 10000 || regAmount > 300000000)
            {
                NLogManager.LogMessage(String.Format("MOMODB ERROR:Amount in valid : {0}", regAmount));
                return JsonMomoResult(-107, "Amount in valid");
            }
            if (String.IsNullOrEmpty(signature))
            {
                return JsonMomoResult(-109, "Signature in valid");
            }
            if (String.IsNullOrEmpty(chargeType))
            {
                return JsonMomoResult(-108, "Method in valid.");
            }

            if (chargeType.ToString() == "momoout")
            {
                if (status == "unknown")
                {
                    int req;
                    try
                    {
                        UpdateStatusMomo(requestId, out req);
                        if (req == 1)
                        {
                            return JsonMomoResult(0, "Success");
                        }
                    }
                    catch (Exception ex)
                    {
                        return JsonMomoResult(0, "Success" + ex.ToString());
                    }
                }
                return JsonMomoResult(0, "RequestID not exits");
            }

            if (!string.IsNullOrEmpty(momoTransId))
            {
                var momoEntity = MOMODAO.Instance.UserMomoRequestGetByRefKey(momoTransId.ToString());
                if (momoEntity != null)
                {
                    return JsonMomoResult(-110, "Record Exist With Transaction");
                }
            }
            //// chargeId = 114997 & chargeType = momo & chargeCode = 98d9db & regAmount = 1000000 & status = success & chargeAmount = 1000000 & requestId = 57014982 & signature = a03d81de8626a87bf0b0a169832eb504
            //string for md5 = 114997momo98d9db1000000success57014982test
            var ApiSecret = "$!52EqgAqVgtWat#";//secretKey
            string content = chargeId.ToString() + chargeType.ToString() + chargeCode.ToString() + chargeAmount.ToString() + status.ToString() + requestId.ToString() + ApiSecret.ToString();

            var sign = MomoChargeApi.MD5(content);
            NLogManager.LogMessage("Co momo_transId +++" + "content: " + content + " - sign: " + sign + "- signature: " + signature);
            if (sign != signature)
            {
                return JsonMomoResult(-104, "Not Authen");
            }
            //MOMODB ERROR:Code:0qxc4yux|ERROR Status :success
            //if (status != "unknown")
            //{
            //NLogManager.LogMessage("accout: " + requestId);

            //NLogManager.LogMessage(String.Format("MOMODB ERROR:Code:{0}|ERROR Status :{1}", code, status));
            //return JsonMomoResult(0, "Success");
            //}


            int PartnerID = 1;
            string PartnerErrorCode = "200";
            long ReceivedMoney = 0;
            long RemainBalance = 0;
            long RequestID = 0;
            double RequestRate = 0;
            int RequestType = 1;
            int Response = 0;
            int outServiceID = 0;
            string mess = requestId.Trim().ToLower();
            Account acount = AccountDAO.Instance.GetAccountInfo(0, mess, null);

            //lấy ra rate  trong dbs 
            double rate = 0;
            var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, "MOMO_B1");
            var firstBanks = bankOperator.FirstOrDefault();
            rate = firstBanks.Rate;

            if (acount != null)
            {
                
                //// rate không hoạt động ở code, tỉ lệ chạy trong store database

                MOMODAO.Instance.UserMomoRequestPartnerCheck(
                    acount.AccountID,
                    RequestType,
                    chargeAmount,
                    PartnerID,
                    null,
                    "1",
                    PartnerErrorCode,
                    chargeCode,
                    null,
                    null, momoTransId.ToString(), momoTransId.ToString(), signature, out Response, out RequestID, out ReceivedMoney, out RemainBalance, out RequestRate, out outServiceID
                );

                if (Response == 1)
                {
                    string msg = "[NẠP] momo: Tài khoản " + acount.AccountName + " nạp số tiền: " + chargeAmount.IntToMoneyFormat();
                    
                    if (chargeAmount < 500000)
                    {
                        SendTelePush(msg, 8);
                    }
                    else
                    {
                        SendTelePush(msg, 8);
                        SendTelePush(msg, 98);
                    }
                    return JsonMomoResult(0, "Success");
                }
                else
                {
                    NLogManager.LogMessage(String.Format("MOMODB ERROR:UserID:{0}|ERROR:{1}", acount.AccountID, Response));
                    return JsonMomoResult(0, "Success");
                }
            }
            else
            {
                NLogManager.LogMessage(String.Format("MOMODB ERROR:User Null Message : {0}", mess));
                return JsonMomoResult(0, "Success");
            }

        }

    }
}