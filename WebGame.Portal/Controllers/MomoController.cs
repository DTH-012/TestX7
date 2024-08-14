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

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/Momo")]
    public class MomoController : BaseApiController
    {
        private int MOMO_MAINTAIN = -8;

        private string MomoType = "momo";
        private static MomoInfoObject PostJsonContent(string sdt, string pHash)
        {
            string sdtmomo = string.Empty;
            string taikhoanmomo = string.Empty;
            long sodutk = 0;
            try
            {
                string url = "https://owa.momo.vn/public/login";
                MomoSendObject root = new MomoSendObject();
                root.user = sdt;
                root.msgType = "USER_LOGIN_MSG";
                root.pass = "123456";
                root.cmdId = "1606757712706000000";
                root.lang = "vi";
                root.time = 1606757712706;
                root.channel = "APP";
                root.appVer = 30011;
                root.appCode = "3.0.1";
                root.deviceOS = "ANDROID";
                root.result = true;
                root.errorCode = 0;
                root.errorDesc = "";
                root.momoMsg = new MomoMsg()
                {
                    _class = "mservice.backend.entity.msg.LoginMsg",
                    isSetup = false
                };
                root.extra = new Extra()
                {
                    pHash = pHash
                };
                byte[] postBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(root));

                var webrequest = System.Net.HttpWebRequest.Create(url);
                webrequest.Method = "POST";
                webrequest.ContentType = "application/json";
                webrequest.ContentLength = postBytes.Length;

                using (var stream = webrequest.GetRequestStream())
                {
                    stream.Write(postBytes, 0, postBytes.Length);
                    stream.Flush();
                }

                var sendresponse = webrequest.GetResponse();
                string sendresponsetext = "";
                using (var streamReader = new StreamReader(sendresponse.GetResponseStream()))
                {
                    sendresponsetext = streamReader.ReadToEnd().Trim();
                }

                if (!string.IsNullOrEmpty(sendresponsetext))
                {
                    JObject momo = JObject.Parse(sendresponsetext);
                    sdtmomo = momo["extra"]["originalPhone"].ToString();
                    taikhoanmomo = momo["extra"]["FULL_NAME"].ToString();
                    sodutk = long.Parse(momo["extra"]["BALANCE"].ToString());
                    return new MomoInfoObject()
                    {
                        MomoPhoneNumber = sdtmomo,
                        MomoUserName = taikhoanmomo,
                        MomoBalance = sodutk
                    };
                }
                NLogManager.LogError("Get momo info json IsNullOrEmpty ");
                return null;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        private MomoInforNew GetJsonMomoInfor(long amount, string displayName)
        {
            //http://sv.caheoxanh.com:10007/api/MM/RegCharge?apiKey=c5b4dccb-ac11-4b80-8972-ad87f16bbc3a&chargeType=momo&amount=10000&requestId=test01
            //

            string apiKey = "c5b4dccb-ac11-4b80-8972-ad87f16bbc3a";// key momo
            string callback_url = "https://napthe.sanhux6.club/api/Momo/MomoCallBackResultAction";
           
            string url = "http://sv.caheoxanh.com:10007/api/MM/RegCharge?apiKey=" + apiKey + "&chargeType=momo" + "&amount=" + amount + "&requestId=" + displayName + "&callback=" + callback_url +  "&usePreMomoTransId=null";

            
            NLogManager.LogMessage("GetJsonMomoInfor : " + url);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                MomoInforNew objReturn = JsonConvert.DeserializeObject<MomoInforNew>(result);
                return objReturn;
            }

            catch (Exception ex)
            {
                MomoInforNew objReturn = new MomoInforNew();
                objReturn.stt = -99;
                NLogManager.PublishException(ex);
                return objReturn;
            }
        }


        public static MomoInfoResponse GetMomoInforRequest()
        {
            String result = string.Empty;
            MomoInfoResponse card = new MomoInfoResponse();
            string url = ConfigurationManager.AppSettings["MOMO_CHARGING_URL"].ToString();
            string momoApiKey = ConfigurationManager.AppSettings["MOMO_API_KEY_MM"].ToString();
            //string signature = CardVinaHelper.GenerateSignature(requestId, cardNumber, serialNumber, VINA_TELCO, cardValue, secretKey);
            String urlParameter = String.Format("{0}", momoApiKey);
            NLogManager.LogMessage("Url Param : " + urlParameter);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(String.Format("{0}{1}", url, urlParameter));

                var postData = string.Empty;
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                if (response != null)
                {
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    card = JsonConvert.DeserializeObject<MomoInfoResponse>(responseString);
                }
                else
                {
                    card.status = 0;
                }
            }
            catch (Exception e)
            {
                card.status = 0;
            }
            return card;
        }
       


        [ActionName("GetInfor")]
        [HttpGet]
        public dynamic GetInfor(long amount)
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
                //random key
                long lngValue;
                try
                {
                    var listP = new List<string>();
                    var listPartners = BankPartnersDAO.Instance.GetList(ServiceID);
                    if (listPartners == null || !listPartners.Any())
                    {
                        return new
                        {
                            ResponseCode = MOMO_MAINTAIN,
                            Message = ErrorMsg.MOMOLOCK + "-1"
                        };
                    }
                    listP = listPartners.Where(c => c.Momo != "0").Select(c => c.Momo).ToList();
                    if (listP == null || !listP.Any())
                    {
                        return new
                        {
                            ResponseCode = MOMO_MAINTAIN,
                            Message = ErrorMsg.MOMOLOCK + "-2"
                        };
                    }
                    var strRandom = String.Join(",", listP);
                    var listAcitve = strRandom.Split(',');
                    var ListIntActive = listAcitve.Select(long.Parse).ToList().Where(c => c > 0).ToList();
                    if (ListIntActive == null || !ListIntActive.Any())
                    {
                        return new
                        {
                            ResponseCode = MOMO_MAINTAIN,
                            Message = ErrorMsg.MOMOLOCK + "-3"
                        };
                    }
                    Random rndActives = new Random();
                    var intRandom = rndActives.Next(0, ListIntActive.Count);
                    lngValue = ListIntActive[intRandom];
                }
                catch
                {
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.MOMOLOCK + "-4"
                    };
                }
                var momoHappy = new List<long> { 1, 2 };
                var momoShopTheNhanh = new List<long> { 3, 4 };
                var monoconfig = string.Empty;
                if (momoHappy.Contains(lngValue))
                {
                    monoconfig = MOMOConfig;

                }
                else if (momoShopTheNhanh.Contains(lngValue))
                {
                    monoconfig = MOMOConfigShopTheNhanh;
                }
                else
                {
                    return new
                    {
                        ResponseCode = -1009,
                        Message = "Không lấy được cấu hình tỉ lệ"
                    };
                }
                // lngValue = 1 hoac 2 Momo
                if (String.IsNullOrEmpty(monoconfig))
                {
                    return new
                    {
                        ResponseCode = -1019,
                        Message = "Không lấy được cấu hình tỉ lệ"
                    };
                }
                //lấy ra rate  trong dbs 
                double rate = 0;
                var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, monoconfig);
                if (bankOperator == null || !bankOperator.Any())
                {
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.MOMOLOCK + "-9",
                    };
                }

                var firstBanks = bankOperator.FirstOrDefault();
                if (firstBanks == null)
                {
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.MOMOLOCK + "-10",
                    };
                }
                if (!firstBanks.Status)
                {
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.MOMOLOCK + "-11",
                    };
                }
                rate = firstBanks.Rate;
                if (rate <= 0)
                {
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = "Không tìm được cấu hình tỉ giá",
                    };
                }

                MomoInforNew momoInfo = GetJsonMomoInfor(amount, displayName);
                if (momoInfo != null)
                {
                    if (momoInfo.stt == 1)
                    {
                        var lsstDisplay = new List<MomoResult> {
                            new MomoResult(10000,rate),
                            new MomoResult(20000,rate),
                            new MomoResult(100000,rate),
                            new MomoResult(200000,rate),
                            new MomoResult(500000,rate),
                            new MomoResult(1000000,rate),
                        };
                        return new
                        {
                            ResponseCode = 1,
                            Orders = new
                            {
                                //Message = displayName.ToLower(),// ten hien thi
                                
                                WalletAccountName = momoInfo.data.phoneName,//
                                //Amount = Int32.Parse(momoInfo.data.amount),   

                                WalletAccount = momoInfo.data.phoneNum,
								Message = momoInfo.data.code,// noi dung


                                Rate = rate,
                                List = lsstDisplay,
                            }
                        };
                    }
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.MOMOLOCK + "-12 MomoController",
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

    public class Data
    {
        public int id { get; set; }
        public string qr_url { get; set; }
        public string payment_url { get; set; }
        public string code { get; set; }
        public string phoneNum { get; set; }
        public string amount { get; set; }
        public string phoneName { get; set; }
        public string chargeType { get; set; }
        public string redirect { get; set; }
        public string bank_provider { get; set; }
        public int timeToExpired { get; set; }
    }

    public class MomoInforNew
    {
        public int stt { get; set; }
        public string msg { get; set; }
        public Data data { get; set; }
    }
}