using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Models.Momo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using TraditionGame.Utilities;
using TraditionGame.Utilities.BanksGateTheNhanh;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.DNA;
using TraditionGame.Utilities.Http;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Momos.Api.Charges;
using TraditionGame.Utilities.MyUSDT.Charges;
using TraditionGame.Utilities.MyUSDT.Models.Charges;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.Utils;
using TraditionGame.Utilities.SmartBanks.Model;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/MopayBank")]
    public class MopayBankController : BaseApiController
    {
        private int MOMO_MAINTAIN = -8;

        private string MomoType = "momo";

        private MopayListBankResponse GetJsonBankListInfor()
        {

            //http://sv.caheoxanh.com:10007/api/Bank/getBankAvailable?apiKey=c5b4dccb-ac11-4b80-8972-ad87f16bbc3a
            //string url = "http://mopay5.vnm.bz:10007/api/Bank/getBankAvailable?apiKey=" + apiKey;

            string apiKey = "c5b4dccb-ac11-4b80-8972-ad87f16bbc3a";//"870ccddb-c8c0-49c9-9d8f-c4456268fc92";
            string url = "http://sv.caheoxanh.com:10007/api/Bank/getBankAvailable?apiKey=" + apiKey;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                MopayListBankResponse objReturn = JsonConvert.DeserializeObject<MopayListBankResponse>(result);

                return objReturn;
            }
            catch (Exception ex)
            {
                MopayListBankResponse objReturn = new MopayListBankResponse();
                objReturn.stt = -99;
                NLogManager.PublishException(ex);
                return objReturn;
            }
        }

        private MopayInfoResponse GetJsonMopayInfor(long Amount, string chargeType, string bankCode, string displayName)
        {

            //http://sv.caheoxanh.com:10007/api/MM/RegCharge?apiKey=c5b4dccb-ac11-4b80-8972-ad87f16bbc3a&chargeType=momo&amount=10000&requestId=test01
            string callback_url = "https://napthe.sanhux6.club/api/MopayBank/BankCallBackResultAction";
            string apiKey = "c5b4dccb-ac11-4b80-8972-ad87f16bbc3a"; //key bank
            string url = "http://sv.caheoxanh.com:10007/api/MM/RegCharge?apiKey=" + apiKey + "&chargeType=" + chargeType +  "&amount=" + Amount + "&requestId=" + displayName + "&subType=" + bankCode + "&callback=" + callback_url + "&usePreMomoTransId=null";

            //if (!String.IsNullOrEmpty(subType))
            //{
            //    url = url + "&subType=" + subType;
            //}

            NLogManager.LogMessage("GetJsonMopayInfor : " + url);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                MopayInfoResponse objReturn = JsonConvert.DeserializeObject<MopayInfoResponse>(result);
                return objReturn;
            }
            catch (Exception ex)
            {
                MopayInfoResponse objReturn = new MopayInfoResponse();
                objReturn.stt = -99;
                NLogManager.PublishException(ex);
                return objReturn;
            }
        }


        [ActionName("GetListBank")]
        [HttpGet]
        public dynamic GetListBank()
        {
            try
            {
                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                var accountId = AccountSession.AccountID;

                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var tkServiceID = AccountSession.ServiceID;
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                var account = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (account == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (account.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }

                MopayListBankResponse momoInfo = GetJsonBankListInfor();
                if (momoInfo != null)
                {
                    if (momoInfo.stt == 1)
                    {
                        return new
                        {
                            ResponseCode = 1,
                            Orders = new
                            {
                                List = momoInfo.data,
                                Message = displayName.ToLower(),
                            }
                        };
                    }
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.MOMOLOCK + "-12 MopayBankController" + momoInfo.stt,
                    };

                }
                return new
                {
                    ResponseCode = MOMO_MAINTAIN,
                    Message = ErrorMsg.MOMOLOCK + "-13",
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = ErrorMsg.InProccessException
                };
            }
        }

        [ActionName("GetBank")]
        [HttpGet]
        public dynamic GetBank(string bankCode, long Amount)
        {
            try
            {
                NLogManager.LogMessage("GetBank : " + bankCode);
                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                var accountId = AccountSession.AccountID;

                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var tkServiceID = AccountSession.ServiceID;
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                var account = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (account == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (account.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }

                MopayInfoResponse momoInfo = GetJsonMopayInfor(Amount, "bank", bankCode, displayName.ToLower());
                if (momoInfo != null)
                {
                    if (momoInfo.stt == 1)
                    {

                        return new
                        {
                            ResponseCode = 1,
                            Orders = new
                            {
                                List = momoInfo.data,
                                Message = displayName.ToLower(),
                            }
                        };
                    }
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.MOMOLOCK + "-12",
                    };

                }
                return new
                {
                    ResponseCode = MOMO_MAINTAIN,
                    Message = ErrorMsg.MOMOLOCK + "-13",
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = ErrorMsg.InProccessException
                };
            }
        }
    }

    public class BankDataAccount
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class MopayListBankResponse
    {
        public int stt { get; set; }
        public string msg { get; set; }
        public List<BankDataAccount> data { get; set; }
    }

    public class MopayData
    {
        public int id { get; set; }
        public string qr_url { get; set; }
        public string payment_url { get; set; }
        public string code { get; set; }
        public string phoneNum { get; set; }
        public int amount { get; set; }
        public string phoneName { get; set; }
        public string chargeType { get; set; }
        public string redirect { get; set; }
        public string bank_provider { get; set; }
        public int timeToExpired { get; set; }
    }

    public class MopayInfoResponse
    {
        public int stt { get; set; }
        public string msg { get; set; }
        public MopayData data { get; set; }
    }
}