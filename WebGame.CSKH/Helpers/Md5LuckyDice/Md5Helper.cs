﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Helpers.Md5LuckyDice
{
    public class Md5Helper
    {
        public static string GenerateKey(long SessionID ,int type, long accountId, int betSide)
        {
            string value = string.Empty;
            if (type == (int)KeyType.Exist)
            {
                value = string.Format("txmd5.{0}:{1}.{2}", SessionID, accountId,
                    betSide == (int)BetSide.Tai ? "xiu" : "tai");
            }
            else if (type == (int)KeyType.TotalBet)
            {
                value = string.Format("txmd5.{0}:totalbet.{1}", SessionID,
                    betSide == (int)BetSide.Tai ? "tai" : "xiu");
            }
            else if (type == (int)KeyType.Turn)
            {
                value = string.Format("txmd5.{0}:total.{1}", SessionID, betSide == (int)BetSide.Tai ? "tai" : "xiu");
            }
            else if (type == (int)KeyType.Result)
            {
                value = string.Format("txmd5.{0}:result", SessionID);
            }
            else if (type == (int)KeyType.Bet)
            {
                value = string.Format("txmd5.{0}:{1}.{2}", SessionID, accountId,
                    betSide == (int)BetSide.Tai ? "tai" : "xiu");
            }
            else if (type == (int)KeyType.Summon)
            {
                value = string.Format("txmd5.{0}:summon", SessionID);
            }

            return value;
        }
    }
}