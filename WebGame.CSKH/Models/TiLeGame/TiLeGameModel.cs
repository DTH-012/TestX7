using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TraditionGame.Utilities.Utils;
namespace MsWebGame.CSKH.Models.TiLeGame
{
 
    public class TiLeGameModel
    {
        public int Model
        {
            get;
            set;
        }
    }
    public class DataPostSetTiLe
    {
        //AccountID,BetSide,Amount,BetTime
        public int Tile { get; set; }
    }
}