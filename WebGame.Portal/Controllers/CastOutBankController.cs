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
using MsWebGame.Portal.Database.DAO;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.Messages;
using WebGame.Payment.Database.DAO;
using TraditionGame.Utilities.Constants;
using System.Text;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/CastOutBank")]
    public class CastOutBankController : BaseApiController
    {
        public int Rate = 0;//Tỷ lệ rút
        [ActionName("GetListBankCode")]
        [HttpPost]
        public dynamic GetListBankCode()
        {
            string url = "http://apk.sanhux6.club/banklist/list.json"; // Hiện danh sách ngân hàng rút tiền
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                MopayListBankResponse objReturn = JsonConvert.DeserializeObject<MopayListBankResponse>(result);
                return new
                {
                    ResponseCode = 1,
                    Description = "",
                    Data = objReturn.data,
                    Rate = this.Rate
                };
            }
            catch (Exception ex)
            {
                MopayListBankResponse objReturn = new MopayListBankResponse();
                objReturn.stt = -99;

                return new
                {
                    ResponseCode = -99,
                    Description = "",
                    Data = objReturn.data,
                    Rate = this.Rate
                };
            }
        }

        [ActionName("ChargeOut")]
        [HttpOptions, HttpPost]
        public dynamic ChargeOut([FromBody] dynamic input)
        {
            long accountId = AccountSession.AccountID;
            string Nickname = AccountSession.AccountName;
            string SoTk = input.SoTk ?? string.Empty;
            string NameTk = input.NameTk ?? string.Empty;
            string Amount = input.Amount ?? string.Empty;
            string BankName = input.BankName ?? string.Empty;
            string BankCode = input.BankCode ?? string.Empty;
            string Otp = input.Otp ?? string.Empty;
            if (string.IsNullOrEmpty(SoTk) || string.IsNullOrEmpty(NameTk) || string.IsNullOrEmpty(Amount) || string.IsNullOrEmpty(BankName) || string.IsNullOrEmpty(BankCode))
            {
                return new
                {
                    ResponseCode = -99,
                    Message = "Dữ liệu gửi lên không đúng"
                };
            }
            Otp = Otp.Trim();
            if (Otp.Length != OTPAPP_LENGTH && Otp.Length != OTPSMS_LENGTH && Otp.Length != OTPSAFE_LENGTH)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.OtpLengthInValid
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
            int resOtp = 0;
            long otpID;
            string otmsg;
            SMSDAO.Instance.ValidOtp(account.AccountID, Otp.Length == OTPSAFE_LENGTH ? account.PhoneSafeNo : account.PhoneNumber, Otp, ServiceID, out resOtp, out otpID, out otmsg);
            if (resOtp != 1)
            {
                return new
                {
                    ResponseCode = -5,
                    Message = Otp.Trim().Length == 6 ? "Otp không đúng" : ErrorMsg.OtpIncorrect
                };
            }
            long Balance;
            int Response;

            long AmountNumber = long.Parse(Amount);




            // kiểm tra user nạp tiền chưa
            string stt = UserDAO.Instance.USER_RECHARGE_CHECK3(accountId);
            if (stt == "0")
            {
                return new
                {
                    Message = "Bạn Chưa Tham Gia NẠP TIỀN, Không Thể Rút Tiền!\nVui Lòng Liên Hệ CSKH!"
                };

            }
            else
            {
                if (AmountNumber < 100000)
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = "Số tiền tối thiếu rút 100.000"
                    };
                }
                CasoutDAO.Instance.Casout_Bank(accountId, this.Rate, Nickname, SoTk, utf8Convert(NameTk).ToUpper(), AmountNumber, BankCode, utf8Convert(BankName), 1, out Balance, out Response);
                if (Response == 1)
                {
                    string msg = "⬅️[RÚT] bank: " + "Tài khoản " + account.AccountName + " tạo yêu cầu rút Bank số tiền: " + AmountNumber.LongToMoneyFormat() + " Về TKBank: " + NameTk;
                    
                    AccountDAO.Instance.GetAccountInOut(accountId, out long TotalRecharge, out long TotalRechargeBank, out long TotalMomo, out long TotalValueInAgency, out long TotalValueOutAgency, out long Top1InBank, out long Top1InMomo, out long Top1InCard, out DateTime DateTop1InBank, out DateTime DateTop1InMomo, out DateTime DateTop1InCard);
                    AccountDAO.Instance.GetAccount_Profit_SUM(accountId, out long TotalBetValue, out long TotalPrizeValue);


                    string IN = "\n➕Tổng Nạp BANK*MOMO*CARD = " + (Convert.ToInt32(TotalRechargeBank) + Convert.ToInt32(TotalMomo) + Convert.ToInt32(TotalRecharge)).IntToMoneyFormat();
                    string OUT = "\n➖Tổng Rút BANK*MOMO = " + (Convert.ToInt32(TotalValueInAgency) + Convert.ToInt32(TotalValueOutAgency)).IntToMoneyFormat();

                    string naptop1 = "\nLẦN NẠP GẦN NHẤT:\n🥇 BANK: " + Top1InBank.LongToMoneyFormat() + " => Ngày: " + DateTop1InBank + "\n🥈 MOMO: " + Top1InMomo.LongToMoneyFormat() + " => Ngày: " + DateTop1InMomo + "\n🥉 CARD: " + Top1InCard.LongToMoneyFormat() + " => Ngày: " + DateTop1InCard;
                    string tonggame = "\nCHƠI GAME\n" + "🎮 Đặt: " + TotalBetValue.LongToMoneyFormat() + "\n💰 Nhận: " + TotalPrizeValue.LongToMoneyFormat() + "\n🏆 Lỗ/Lãi: " + (Convert.ToInt32(TotalPrizeValue) - Convert.ToInt32(TotalBetValue)).IntToMoneyFormat();
                    msg += IN;
                    msg += OUT;
                    msg += naptop1;
                    msg += tonggame;
                    SendTelePushTelegramID(12, msg, 0, false, account.AccountName);
                    return new
                    {
                        ResponseCode = 1,
                        Message = "Yêu cầu rút Bank thành công!",
                        Balance = Balance,
                    };
                }
                else if (Response == -504)
                {
                    return new
                    {
                        ResponseCode = Response,
                        Message = "Số tiền của bạn không đủ"
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = Response,
                        Message = "Có lỗi xảy ra vui lòng thử lại"
                    };
                }
            }



        }
        [ActionName("Momo")]
        [HttpOptions, HttpPost]
        public dynamic Momo([FromBody] dynamic input)
        {
            long accountId = AccountSession.AccountID;
            string Nickname = AccountSession.AccountName;
            string Phone = input.Phone ?? string.Empty;
            string Amount = input.Amount ?? string.Empty;
            string Name = input.Name ?? string.Empty;
            string Otp = input.Otp ?? string.Empty;
            if (string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(Amount) || string.IsNullOrEmpty(Name))
            {
                return new
                {
                    ResponseCode = -99,
                    Message = "Dữ liệu gửi lên không đúng"
                };
            }
            Otp = Otp.Trim();
            if (Otp.Length != OTPAPP_LENGTH && Otp.Length != OTPSMS_LENGTH && Otp.Length != OTPSAFE_LENGTH)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.OtpLengthInValid
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
            int resOtp = 0;
            long otpID;
            string otmsg;
            SMSDAO.Instance.ValidOtp(account.AccountID, Otp.Length == OTPSAFE_LENGTH ? account.PhoneSafeNo : account.PhoneNumber, Otp, ServiceID, out resOtp, out otpID, out otmsg);
            if (resOtp != 1)
            {
                return new
                {
                    ResponseCode = -5,
                    Message = "Otp không đúng"//Otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                };
            }
            long Balance;
            int Response;

            long AmountNumber = long.Parse(Amount);

            //kiểm tra user nạp tiền chưa
            string stt = UserDAO.Instance.USER_RECHARGE_CHECK3(accountId);
            if (stt == "0")
            {
                return new
                {
                    Message = "Bạn Chưa Tham Gia NẠP TIỀN, Không Thể Rút Tiền!\nVui Lòng Liên Hệ CSKH!"
                };

            }
            else
            {
                if (AmountNumber < 100000)
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = "Số tiền tối thiếu rút 100.000"
                    };
                }
                CasoutDAO.Instance.Casout_Momo(accountId, Nickname, this.Rate, Phone, utf8Convert(Name).ToUpper(), AmountNumber, 1, out Balance, out Response);
                if (Response == 1)
                {
                    string msg = "⬅️[RÚT] momo: Tài khoản " + account.AccountName + " tạo yêu cầu rút Momo số tiền :" + AmountNumber.LongToMoneyFormat() + " vào TKMomo: " + Name;
                    AccountDAO.Instance.GetAccountInOut(accountId, out long TotalRecharge, out long TotalRechargeBank, out long TotalMomo, out long TotalValueInAgency, out long TotalValueOutAgency,out long Top1InBank, out long Top1InMomo, out long Top1InCard, out DateTime DateTop1InBank, out DateTime DateTop1InMomo, out DateTime DateTop1InCard);
                    AccountDAO.Instance.GetAccount_Profit_SUM(accountId, out long TotalBetValue, out long TotalPrizeValue);


                    string IN = "\n➕Tổng Nạp BANK*MOMO*CARD = " + (Convert.ToInt32(TotalRechargeBank) + Convert.ToInt32(TotalMomo) + Convert.ToInt32(TotalRecharge)).IntToMoneyFormat() ;
                    string OUT = "\n➖Tổng Rút BANK*MOMO = " + (Convert.ToInt32(TotalValueInAgency) + Convert.ToInt32(TotalValueOutAgency)).IntToMoneyFormat();

                    string naptop1 = "\nLẦN NẠP GẦN NHẤT:\n🥇 BANK: " + Top1InBank.LongToMoneyFormat() + " => Ngày: " + DateTop1InBank + "\n🥈 MOMO: " + Top1InMomo.LongToMoneyFormat() + " => Ngày: " + DateTop1InMomo + "\n🥉 CARD: " + Top1InCard.LongToMoneyFormat() + " => Ngày: " + DateTop1InCard;
                    string tonggame = "\nCHƠI GAME\n" + "🎮 Đặt: " + TotalBetValue.LongToMoneyFormat() + "\n💰 Nhận: " + TotalPrizeValue.LongToMoneyFormat() + "\n🏆 Lỗ/Lãi: " + (Convert.ToInt32(TotalPrizeValue) - Convert.ToInt32(TotalBetValue)).IntToMoneyFormat();
                    msg += IN;
                    msg += OUT;
                    msg += naptop1;
                    msg += tonggame;
                    SendTelePushTelegramID(11, msg, 0, false, account.AccountName);
                    return new
                    {
                        ResponseCode = 1,
                        Message = "Yêu cầu rút tiền Momo thành công!",
                        Balance = Balance,
                    };
                }
                else if (Response == -504)
                {
                    return new
                    {
                        ResponseCode = Response,
                        Message = "Số tiền của bạn không đủ"
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = Response,
                        Message = "Có lỗi xảy ra vui lòng thử lại"
                    };
                }
            }



        }
        [ActionName("GetMomo")]
        [HttpPost]
        public dynamic GetMomo()
        {
            return new
            {
                ResponseCode = 1,
                Description = "",
                Rate = this.Rate
            };
        }

        public string utf8Convert(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }
    }
 
}