﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace MsWebGame.CSKH.Models.SicboChanLeLuckyDice
{
    public class SicboChanLeRedisSession
    {
        public long SessionID { get; set; }
        public MsWebGame.CSKH.Helpers.SicboChanLeLuckyDice.GameState CurrentState { get; set; }
        public int Ellapsed { get; set; }
        public int Dice1 { get; set; }
        public int Dice2 { get; set; }
        public int Dice3 { get; set; }

        public SicboChanLeRedisSession() { }

        public SicboChanLeRedisSession(long sessionId, MsWebGame.CSKH.Helpers.SicboChanLeLuckyDice.GameState currState, int ellapsed, int dice1, int dice2, int dice3)
        {
            this.SessionID = sessionId;
            this.CurrentState = currState;
            this.Ellapsed = ellapsed;
            this.Dice1 = dice1;
            this.Dice2 = dice2;
            this.Dice3 = dice3;
        }
    }
}