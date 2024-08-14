using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TraditionGame.Utilities;
using MsWebGame.CSKH.Database.DTO;
namespace MsWebGame.CSKH.Database.DAO
{
    public class CcuDAO
    {
        private static readonly Lazy<CcuDAO> _instance = new Lazy<CcuDAO>(() => new CcuDAO());

        public static CcuDAO Instance
        {
            get { return _instance.Value; }
        }
        public List<CuuListModel> GetLists(DateTime DateStart, DateTime DateEnd)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);

                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_DateStart", DateStart));
                param.Add(new SqlParameter("@_DateEnd", DateEnd));
                var lstRs = db.GetListSP<CuuListModel>("SP_CCU_GetList", param.ToArray());
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
            return null;
        }

    }
}