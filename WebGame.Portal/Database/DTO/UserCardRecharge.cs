﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class UserCardRecharge
    {
        public long RequestID { get; set; }

        public long UserID { get; set; }

        public int TelOperatorID { get; set; }

        public int CardID { get; set; }

        public string CardNumber { get; set; }

        public string SerialNumber { get; set; }

        public int CardValue { get; set; }

        public double TeleRate { get; set; }

        public long? ReceivedMoney { get; set; }

        public int Status { get; set; }
        public double Rate { get; set; }



        public string Description { get; set; }

        public string OperatorCode { get; set; }
        public string PartnerErrorCode { get; set; }


        public DateTime CreateDate { get; set; }



        public string StatusStr
        {
            get
            {
                if (Status ==0)
                {
                    return "Chờ xử lý";
                }else if (Status == -1)
                {
                    return "Thất bại";
                }
                else if (Status == -2)
                {
                    return "Thất bại";
                }
                else if (Status == -3)
                {
                    return "Thất bại-SMG";
                }
                else if (Status == 2)
                {
                    return "Thành công";
                }
                else if (Status == 1)
                {
                    return "Chờ xử lý";
                }
                else if (Status == 3)
                {
                    return "Chờ xử lý";
                }
                else if (Status == 4)
                {
                    return "Thành công-SMG";
                }
                else 
                {
                    return "Thất bại";
                }
            }
        }
    }
}