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
using System.Drawing;
using TraditionGame.Utilities.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/MopayBank")]
    public class MopayBankController : BaseApiController
    {
        //private string MOMO_SECRETKEY = ConfigurationManager.AppSettings["MOMO_SECRETKEY"].ToString();
        //private List<string> AccpetProvider = new List<string>() { "MMO" };

        public static void UpdateStatus(string @_requestId, out int Response)
        {
            DBHelper db = null;
            Response = -99;
            try
            {
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

                db.ExecuteNonQuerySP("SP_Casout_Bank_UpdateStatus", param.ToArray());

                Response = Convert.ToInt32(param[4].Value);

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

        [ActionName("BankCallBackResultAction")]
        [HttpGet]

        public HttpResponseMessage BankCallBackResultAction(int chargeId, string chargeType, string chargeCode, string status, int regAmount,int chargeAmount, string momoTransId, string requestId, string signature)
        {
            // string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            // if (string.IsNullOrEmpty(ip))
            //  {
            //      ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            // }

            //  string IpsBank = System.Configuration.ConfigurationManager.AppSettings["IpsBank"];
            //  NLogManager.LogMessage("BankCallBackResult-IP: " + ip + " IpsBank:" + IpsBank);
            //  if (!string.IsNullOrEmpty(IpsBank) && ip != IpsBank)
            //  {
            //     var msg = String.Format("{0}|{1}", "-1", "Ip Invalid");
            //     return JsonMomoResult(-106, msg);
            //  }
            //if (momoRequest == null)
            //{
            //    return JsonMomoResult(-100, "ParaInvalid model null");
            //}
            //LoggerHelper.LogUSDTMessage(String.Format("MomoCharge.CallBackResult CallBackModel:{0}", JsonConvert.SerializeObject(momoRequest)));

            //if (string.IsNullOrEmpty(code))
            //{
            //    return JsonMomoResult(-105, "MomoId  null");
            //}


            if (String.IsNullOrEmpty(chargeCode))
            {
                return JsonMomoResult(-106, "Message null");
            }
            if (chargeAmount <= 0 || chargeAmount < 10000 || chargeAmount > 300000000)
            {
                NLogManager.LogMessage(String.Format("MOMODB ERROR:Amount in valid : {0}", chargeAmount));
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
            if (String.IsNullOrEmpty(requestId))
            {
                return JsonMomoResult(-105, "RequestID in valid.");
            }

            if (chargeType.ToString() == "bankout")
            {
                if (status == "success")
                {
                    int req;
                    try
                    {
                        UpdateStatus(requestId, out req);
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
            var bankEntity = USDTDAO.Instance.UserBankRequestGetByCodeFromPartner(1, chargeCode.ToString());
            if (bankEntity != null)
            {
                return JsonMomoResult(-110, "Record Exist With Transaction");
            }
            var ApiSecret = "$!52EqgAqVgtWat#";//secretKey

            //your_url?chargeId=114997&chargeType=momo&chargeCode=98d9db&regAmount=1000000&status=success&chargeAmount=1000000&requestId=57014982&signature=a03d81de8626a87bf0b0a169832eb504với PasswordLv2 = test
            //string for md5 = 114997momo98d9db1000000success57014982test
            string content = chargeId.ToString() + chargeType.ToString() + chargeCode.ToString() + chargeAmount.ToString() + status.ToString() + requestId.ToString() + ApiSecret.ToString();
            var sign = MomoChargeApi.MD5(content);
            //NLogManager.LogMessage("momo_transId: " + momo_transId + "content: " + content + " - sign: " + sign + "- signature: " + signature);
            if (sign != signature)
            {
                return JsonMomoResult(-104, "Not Authen");
            }


            //lấy ra rate  trong dbs 
            double rate = 0;
            var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, "BANK_B1");
            var firstBanks = bankOperator.FirstOrDefault();
            rate = firstBanks.Rate;



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
            if (acount != null)
            {


                /*MOMODAO.Instance.UserMomoRequestPartnerCheck(
                    acount.AccountID, RequestType, amount, PartnerID, null,
                    "1", PartnerErrorCode,
                    code, null,
                    null, momoTransId.ToString(), momoTransId.ToString(), signature, out Response, out RequestID, out ReceivedMoney, out RemainBalance, out RequestRate, out outServiceID
                );*/

                if (status == "success")
                {
                    Bank_Success = 1;
                }
                else
                {
                    Bank_Success = 0;
                }
                long bonusAmount = 0;
                bonusAmount = Convert.ToInt64(1 * chargeAmount); // rate: tỉ lệ nạp
                //if (finish_amount >= 100000 && finish_amount < 500000)
                //{
                //    bonusAmount = Convert.ToInt64(1.5 * finish_amount); // rate: tỉ lệ nạp
                //}
                //else if (finish_amount >= 500000)
                //{
                //    bonusAmount = Convert.ToInt64(2 * finish_amount); // rate: tỉ lệ nạp
                //}
                //else
                //{
                //    bonusAmount = Convert.ToInt64(1 * finish_amount); // rate: tỉ lệ nạp
                //}
                //string stt = UserDAO.Instance.USER_RECHARGE_CHECK3(acount.AccountID);
                //NLogManager.LogMessage("0: chưa nạp, 1: đã nạp => " + stt);
                //if (stt == "0")
                //{
                //    if (finish_amount >= 100000 && finish_amount < 500000)
                //    {
                //        bonusAmount = Convert.ToInt64(1.5 * finish_amount); // rate: tỉ lệ nạp
                //    }
                //    else if (finish_amount >= 500000)
                //    {
                //        bonusAmount = Convert.ToInt64(2 * finish_amount); // rate: tỉ lệ nạp
                //    }

                //}
                //else
                //{
                //    bonusAmount = finish_amount;
                //}


                USDTDAO.Instance.UserBankRequestPartnerCheckMopay(
                    chargeCode,
                    acount.AccountID,
                    Bank_Success,
                    ServiceID,
                    0,
                    0,
                    CHECKER_ID,
                    bonusAmount,
                    chargeAmount,
                    chargeAmount,
                    status, out RemainBalance, out Response);


                if (Response == 1)
                {
                    //var dnaHelper = new DNAHelpers(outServiceID, DNA_PLATFORM);
                    //dnaHelper.SendTransactionPURCHASE(acount.AccountID, 10, null, amount, amount);

                    string msg = "[NẠP] bank: Tài khoản " + acount.AccountName + " nạp số tiền: " + chargeAmount.IntToMoneyFormat();

                    //string msg = "Nạp Bank: " +  "Tài khoản " + acount.AccountName + " Nạp Bank số tiền :" + finish_amount;
                    if (chargeAmount < 500000)
                    {
                        SendTelePush(msg, 2);
                    }
                    else
                    {
                        SendTelePush(msg, 2);
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