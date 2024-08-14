using MsWebGame.Portal.Database.DAO;
using MsWebGame.RedisCache.Cache;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Session;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/Esports")]
    public class EsportsController : BaseApiController
    {
        public string channel_key = "HkhNY4xtUTX8b2Jm";
        [HttpPost]
        [Route("GetAccountInfo")]
        public dynamic GetAccountInfo()
        {
            try
            {
                var displayName = AccountSession.AccountName;
                EsportsDataBalance objReturn = Esports.GetBalance(displayName);
                return new
                {
                    ResponseCode = 1,
                    Message = "",
                    SabeCoin = objReturn.GetSabeCoin(),
                    CasinoCoin = objReturn.GetCasinoCoin()
                };
            }
            catch (Exception ex)
            {
                
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
            
        }
        
        [Route("GetLink")]
        [HttpOptions, HttpPost]
        public dynamic GetLink([FromBody] dynamic input)
        {
            string platform = "web-mobile";
            try {

               platform = input.platform;
            }
            catch (Exception ex)
            {
                platform = "web-mobile";
                NLogManager.PublishException(ex);

            }
            NLogManager.Debug(platform);

            try
            {

                var displayName = AccountSession.AccountName;
                int device  = AccountSession.DeviceType;
                string url = "http://127.0.0.1:5000/ApiGaming/getLink?username=" + displayName + "&channel_key=" + channel_key + "&platform=" + platform;
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                NLogManager.Debug(result);
                EsDataUrl objReturn = JsonConvert.DeserializeObject<EsDataUrl>(result);
                if (objReturn != null && objReturn.error_code == 1)
                {
                    
                    return new
                    {
                        ResponseCode = 1,
                        Message = "",
                        SabeLink = device == 1? new string[] { objReturn.GetUrlSabaDesktop(), objReturn.GetUrlSabaMobile() } : new string[] { objReturn.GetUrlSabaMobile(), objReturn.GetUrlSabaDesktop() },
                        CasinoLink = new string[] { objReturn.GetUrlCasino() },
                    };
                }
                
            }
            catch (Exception ex)
            {
              
                NLogManager.PublishException(ex);
               
            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }
        [HttpPost]
        [Route("Deposit")]
        public dynamic Deposit(EsPostData esPost)
        {
            //return new
            //{
            //    ResponseCode = -100,
            //    Message = "Chức năng đang bảo trì"
            //};
            try
            {
                var displayName = AccountSession.AccountName;

                long accountId = AccountSession.AccountID;

                string keyHu = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + accountId);

                try
                {
                    MsWebGame.Portal.Models.ParConfigLiveSport parConfigLiveSport = new Models.ParConfigLiveSport();
                    RedisCacheProvider _cachePvd = new RedisCacheProvider();

                    if (_cachePvd.Exists(keyHu))
                    {

                        parConfigLiveSport = _cachePvd.Get<MsWebGame.Portal.Models.ParConfigLiveSport>(keyHu);
                        if (parConfigLiveSport != null && parConfigLiveSport.Game.Count > 0 && parConfigLiveSport.Game.Contains(parConfigLiveSport.GameId))
                        {
                            return new
                            {
                                ResponseCode = 0,
                                Message = "Tài khoản bị hạn chế"
                            };
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    
                }

                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                long balance = 0;
                long safebalance = 0;

                AccountDAO.Instance.GetBalance(accountId, out balance, out safebalance);

                if (balance >= esPost.Amount)
                {
                    if (esPost.Amount >= 100000)
                    {
                        
                        string url = "http://127.0.0.1:5000/ApiGaming/deposit?username=" + displayName + "&channel_key=" + channel_key + "&wallet=" + esPost.GetWallet() + "&amount=" + esPost.GetAmountGameToSaba();
                        var request = (HttpWebRequest)WebRequest.Create(url);
                        request.ContentType = "application/json";
                        request.Method = "GET";//GET
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        response.Close();
                        NLogManager.Debug(url);
                        NLogManager.Debug(result);
                        EsResponse objReturn = JsonConvert.DeserializeObject<EsResponse>(result);
                        if (objReturn != null)
                        {
                            if (objReturn.error_code == 1)
                            {
                               
                                long Balance = 0;
                                int response_id = 0;
                                EsportsDAO.Instance.Esports_Deposit_To_Game(esPost.GetWallet(), accountId, Convert.ToInt64(esPost.Amount), 1, out Balance, out response_id);
                                if (response_id == 1)
                                {
                                    EsportsDataBalance esportsData = Esports.GetBalance(displayName);

                                    string msg = "[Deposit] Tài khoản : " + displayName + " Vừa nạp " + esPost.Amount + " Vào ví , Số dư ví:"+ esportsData.GetSabeCoin()+" Số dư game:"+ Balance;
                                    SendTelePush(msg, 10);


                                    return new
                                    {
                                        ResponseCode = 1,
                                        Message = "Giao dịch thành công!",
                                        Balance = Balance,
                                        EsportBalance = esportsData.GetSabeCoin()
                                    };
                                }
                                else
                                {
                                    return new
                                    {
                                        ResponseCode = 1,
                                        Message = result+" "+ response_id,//"Có lỗi xảy ra. Liên hệ với admin :200",
                                        Info = "00000"
                                    };
                                }
                            }
                            else
                            {
                                return new
                                {
                                    ResponseCode = 1,
                                    Message = result, //"Có lỗi xảy ra. Liên hệ với admin :200",
                                    Info = "11111"
                                };
                            }
                            
                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = 1,
                                Message = result,//"Có lỗi xảy ra. Liên hệ với admin Code :100",
                                Info = "22222"
                            };
                        }
                    }
                    else
                    {
                        return new
                        {
                            ResponseCode = -1,
                            Message = "Số tiền giao dịch phải từ 100K G trở lên",
                        };
                    }
                }
                else
                {
                    return new
                    {
                        ResponseCode = -1,
                        Message = "Số tiền của bạn không đủ để thực hiện giao dịch",
                    };
                }
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ex.Message
                };
            }
           
        }
        [HttpPost]
        [Route("Withdrawal")]
        public dynamic Withdrawal(EsPostData esPost)
        {
            //return new
            //{
            //    ResponseCode = -100,
            //    Message = "Chức năng đang bảo trì"
            //};
            try
            {
                var displayName = AccountSession.AccountName;

                long accountId = AccountSession.AccountID;
                string keyHu = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + accountId);

                try
                {
                    MsWebGame.Portal.Models.ParConfigLiveSport parConfigLiveSport = new Models.ParConfigLiveSport();
                    RedisCacheProvider _cachePvd = new RedisCacheProvider();

                    if (_cachePvd.Exists(keyHu))
                    {

                        parConfigLiveSport = _cachePvd.Get<MsWebGame.Portal.Models.ParConfigLiveSport>(keyHu);
                        if (parConfigLiveSport != null && parConfigLiveSport.Game.Count > 0 && parConfigLiveSport.Game.Contains(parConfigLiveSport.GameId))
                        {
                            return new
                            {
                                ResponseCode = 0,
                                Message = "Tài khoản bị hạn chế"
                            };
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                EsportsDataBalance Return = Esports.GetBalance(displayName);
                bool Oke = false;
                double Coin = 0;
                double Amount = 0;
                if (esPost.GetWallet() == "saba")
                {
                    if (Return.GetSabeCoin() >= esPost.Amount)
                    {
                        Oke = true;
                        Coin = esPost.Amount;
                        Amount = esPost.GetAmountSabaToGame();
                    }
                }
                else
                {
                    if (Return.GetCasinoCoin() >= esPost.Amount)
                    {
                        Oke = true;
                        Coin = esPost.Amount;
                        Amount = esPost.GetAmountCasinoToGame();
                    }
                }
                if (Oke)
                {
                    
                    string url = "http://127.0.0.1:5000/ApiGaming/withdrawal?username=" + displayName + "&channel_key=" + channel_key+ "&wallet=" + esPost.GetWallet() + "&amount=" + Coin;
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "GET";//GET
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    response.Close();
                    NLogManager.Debug(result);
                    EsResponse objReturn = JsonConvert.DeserializeObject<EsResponse>(result);
                    if (objReturn != null && objReturn.error_code == 1)
                    {

                        long Balance = 0;
                        int response_id = 0;
                        EsportsDAO.Instance.SP_Esports_Withdraw_To_Game(esPost.GetWallet(), accountId, Convert.ToInt64(Amount), 1, out Balance, out response_id);
                        if (response_id == 1)
                        {
                            EsportsDataBalance GetBalance = Esports.GetBalance(displayName);

                            string msg = "[Withdrawal] Tài khoản : " + displayName + " Rút về game " + esPost.Amount + " Ví : Số dư ví :" + GetBalance.GetSabeCoin() + " , Số dư game:" + Balance;
                            SendTelePush(msg, 10);

                            return new
                            {
                                ResponseCode = 1,
                                Message = "Giao dịch thành công!",
                                Balance = Balance,
                                EsportBalance = GetBalance.GetSabeCoin()
                            };
                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = 1,
                                Message = "Có lỗi xảy ra. Liên hệ với admin",
                            };
                        }
                    }
                }
                else
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = "Tiền của bạn không đủ để thực hiện giao dịch!",
                    };
                }

                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };

            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ex.Message
                };

            }
            
        }
    }
    public class Esports
    {
        private static string channel_key = "HkhNY4xtUTX8b2Jm";
        public static EsportsDataBalance GetBalance(string displayName)
        {
            try
            {
               
                string url = "http://127.0.0.1:5000/ApiGaming/getBalance?username=" + displayName + "&channel_key=" + channel_key;
                NLogManager.Debug(url);
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
              
                EsportsDataBalance objReturn = JsonConvert.DeserializeObject<EsportsDataBalance>(result);
                return objReturn;
            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
                return new EsportsDataBalance();
            }
        }
    }
    public class EsportsDataBalance
    {
        public class EsportsInfo
        {
            public string name { set; get; }
            public double balance { set; get; }
        }
        // //{"error_code":1,"message":"success","data":[{"name":"Casino","balance":0},{"name":"Saba","balance":0}]}
        public int error_code { set; get; } = -1;
        public string message { set; get; } = "none";
        public List<EsportsInfo> data { set; get; } = new List<EsportsInfo>();
        public double GetSabeCoin()
        {
            EsportsInfo esportsInfo = data.Where(e => { return e.name == "Saba"; }).FirstOrDefault();
            return esportsInfo != null ? esportsInfo.balance : 0;
        }
        public double GetCasinoCoin()
        {
            EsportsInfo esportsInfo = data.Where(e => { return e.name == "Casino"; }).FirstOrDefault();
            return esportsInfo != null ? esportsInfo.balance : 0;
        }
    }
    public class EsDataUrl
    {
        public int error_code { set; get; } = -1;
        public string message { set; get; } = "none";
        public Dictionary<string,string> data { set; get; } = new Dictionary<string, string>();

        public string GetUrlSabaDesktop()
        {
            return data.ContainsKey("url_saba_desktop") ? data ["url_saba_desktop"] : "#";
        }
        public string GetUrlSabaMobile()
        {
            return data.ContainsKey("url_saba_mobile") ? data["url_saba_mobile"] : "#";
        }
        public string GetUrlCasino()
        {
            return data.ContainsKey("url_casino") ? data["url_casino"] : "#";
        }
        //{"error_code":1,"message":"success","data":{"url_saba_desktop":"http://mkt.a0366.absport.fun/deposit_processlogin.aspx?lang=vn&token=TbjOZnHDE7uFIUgX7FeA","url_saba_mobile":"http://ismart.a0366.absport.fun/deposit_processlogin.aspx?lang=vn&token=TbjOZnHDE7uFIUgX7FeA","url_casino":"https://wmvn.wm2088.net?sid=086014E80003A168M731554&ui=0&lang=vi&voice=vi&co=wm&stream=hls"}}
    }
    public class EsPostData
    {
        public string Wallet { set; get; }
        public double Amount { set; get; }
        public string Type { set; get; }
        public string GetWallet()
        {
            return this.Wallet == "SabaEsports"? "saba" : "casino";
        }
        public double GetAmountGameToSaba()
        {
            return Amount * 0.00083d;
        }
        public double GetAmountSabaToGame()
        {
            return Amount / 0.00083d;
        }
        public double GetAmountGameToCasino()
        {
            return Amount * 0.00083d;
        }
        public double GetAmountCasinoToGame()
        {
            return Amount / 0.00083d;
        }
    }
    public class EsResponse
    {
        public int error_code { set; get; } = -1;
        public string message { set; get; } = "none";
    }
}