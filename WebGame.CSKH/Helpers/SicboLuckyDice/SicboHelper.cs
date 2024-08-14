using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Helpers.SicboLuckyDice
{
    public class SicboHelper
    {
        public static string GenerateKey(long SessionID ,int type, long accountId, int betSide)
        {
            string value = string.Empty;
            if (type == (int)KeyType.Exist)
            {
                value = string.Format("sicbo.{0}:{1}.{2}", SessionID, accountId,
                    betSide == (int)BetSide.Tai ? "xiu" : "tai");
            }
            else if (type == (int)KeyType.TotalBet)
            {
                value = string.Format("sicbo.{0}:totalbet.{1}", SessionID,
                    betSide == (int)BetSide.Tai ? "tai" : "xiu");
            }
            else if (type == (int)KeyType.Turn)
            {
                value = string.Format("sicbo.{0}:total.{1}", SessionID, betSide == (int)BetSide.Tai ? "tai" : "xiu");
            }
            else if (type == (int)KeyType.Result)
            {
                value = string.Format("sicbo.{0}:result", SessionID);
            }
            else if (type == (int)KeyType.Bet)
            {
                if (betSide == 0)
                {
                    value = string.Format("sicbo.{0}:{1}.{2}", SessionID, accountId, "tai");
                }
                else if (betSide == 1)
                {
                    value = string.Format("sicbo.{0}:{1}.{2}", SessionID, accountId, "xiu");
                }
                else if (betSide == 2)
                {
                    value = string.Format("sicbo.{0}:{1}.{2}", SessionID, accountId, "chan");
                }
                else if (betSide == 3)
                {
                    value = string.Format("sicbo.{0}:{1}.{2}", SessionID, accountId, "le");
                }
            }
            else if (type == (int)KeyType.Summon)
            {
                value = string.Format("sicbo.{0}:summon", SessionID);
            }

            return value;
        }
    }
}