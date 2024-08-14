﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class UserRank
    {

        public long UserID { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName  { get;set;}
        public int Avatar { get; set; }
        public long RankID { get; set; }
        public long VP { get; set; }
        public string RankName { get; set; }
    }
}