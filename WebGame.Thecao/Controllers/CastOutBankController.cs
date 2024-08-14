using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TraditionGame.Utilities.MyUSDT.Models.Exchanges;
using System.IO;

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/CastOutBank")]
    public class CastOutBankController
    {
        [ActionName("GetListBankCode")]
        [HttpPost]
        public dynamic GetListBankCode()
        {
            string apiKey = "b0920e81-4341-44f0-81a8-033d65ca8632"; //870ccddb-c8c0-49c9-9d8f-c4456268fc92
            string url = "http://mopay5.vnm.bz:10007/api/Bank/getListBankCode?apiKey=" + apiKey;
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
                
                return objReturn;
            }
        }

        [ActionName("ChargeOut")]
        [HttpPost]
        public dynamic ChargeOut()
        {
            return null;
        }
        [ActionName("CallBackResult")]
        [HttpPost]
        public dynamic CallBackResult([FromBody] CallBackSellorderRequestModel model)
        {
            return null;
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
}