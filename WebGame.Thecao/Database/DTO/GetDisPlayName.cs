using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO
{
    public class GetDisPlayName
    {
        public long UserID { get; set; }

        public string Username { get; set; }

        public string DisplayName { get; set; }
    }
}