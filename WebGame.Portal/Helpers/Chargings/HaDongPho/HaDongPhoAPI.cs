using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using MsWebGame.Portal.Helpers.Chargings.Cards;
using MsWebGame.Portal.ShopMuaThe;
using Newtonsoft.Json;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Helpers.Chargings.HaDongPho
{
    public class HaDongPhoAPI
    {
        public static HaDongPhoResponse SendRequest(string requestId, string cardNumber, string serialNumber, int cardValue, string cardtype)
        {
            String result = string.Empty;
            HaDongPhoResponse card = new HaDongPhoResponse();
            string apiKey = "c5b4dccb-ac11-4b80-8972-ad87f16bbc3a"; // key card

            //http://sv.caheoxanh.com:10007/api/SIM/RegCharge?apiKey=c5b4dccb-ac11-4b80-8972-ad87f16bbc3a&type=vt&code=xxxxxxxx&serial=yyyyyy&menhGia=10000&requestId=test01

            string callback_url = "https://napthe.sanhux6.club/api/CardChargingHDP/CallBackResult";

            string url = "http://sv.caheoxanh.com:10007/api/SIM/RegCharge?apiKey=" + apiKey + "&type=" + cardtype + "&code=" + cardNumber + "&serial=" + serialNumber + "&menhGia=" + cardValue + "&requestId=" + requestId + "&callback=" + callback_url;

            string urlParameter = new JavaScriptSerializer().Serialize(new
            {
                requestid = requestId,
                serial = serialNumber,
                pincode = cardNumber,
                telco = cardtype,
                amount = cardValue

            });
            NLogManager.LogMessage("Url Param : " + urlParameter);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var resulrt = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                HaDongPhoResponse objReturn = JsonConvert.DeserializeObject<HaDongPhoResponse>(resulrt);
                return objReturn;

            }
            catch (Exception e)
            {
                card.errorcode = "-99";
            }

            return card;
        }
    }
}