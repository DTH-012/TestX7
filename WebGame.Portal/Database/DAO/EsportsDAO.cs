using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class EsportsDAO
    {
        private static readonly Lazy<EsportsDAO> _instance = new Lazy<EsportsDAO>(() => new EsportsDAO());

        public static EsportsDAO Instance
        {
            get { return _instance.Value; }
        }
        public void Esports_Deposit_To_Game(string Wallet, long UserID, long Amount, int ServiceID, out long Balance, out int Response)
        {
            DBHelper db = null;
            Balance = 0;
            Response = -99;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = UserID;

                param[1] = new SqlParameter("@_Wallet", SqlDbType.NVarChar);
                param[1].Size = 20;
                param[1].Value = Wallet;

                param[2] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[2].Value = Amount;

                param[3] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[3].Value = ServiceID;

                param[4] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;

                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Esports_Deposit_To_Game", param.ToArray());

                Balance = Convert.ToInt64(param[4].Value);
                Response = Convert.ToInt32(param[5].Value);
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
        }
        public void SP_Esports_Withdraw_To_Game(string Wallet,long UserID, long Amount, int ServiceID,out long Balance,  out int Response)
        {
            DBHelper db = null;
            Balance = 0;
            Response = -99;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = UserID;

                param[1] = new SqlParameter("@_Wallet", SqlDbType.NVarChar);
                param[1].Size = 20;
                param[1].Value = Wallet;

                param[2] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[2].Value = Amount;

                param[3] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[3].Value = ServiceID;

                param[4] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;

                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Esports_Withdraw_To_Game", param.ToArray());

                Balance = Convert.ToInt64(param[4].Value);
                Response = Convert.ToInt32(param[5].Value);
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
             
        }
    }
}